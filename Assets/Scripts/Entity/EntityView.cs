using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityView : MonoBehaviour, IEntityScanner, IProjectileSpawner, IPathfinder, IMover, ILooker
{
    [Header("Ground Detection")]
    [SerializeField] private Collider2D _groundCheck;      // 足元のオブジェクト
    [SerializeField] private protected EntityEffectController _entityEffectController;
    [SerializeField] private protected SpriteRenderer _spriteRenderer;

    [Header("Scan")]
    [SerializeField] private Transform _eyePosition;      // 視線の始点（子オブジェクト）

    private EntityWorldUI _worldUI => _entityEffectController.WorldUI;
    private ParticleSystem _par_onShieldDMG => _entityEffectController.Par_onShieldDMG;
    private ParticleSystem _par_onHPDMG => _entityEffectController.Par_onHPDMG;

    private protected Rigidbody2D _rb;

    private NavPathfinder _navPathfinder;
    public NavPathfinder NavPathfinder => _navPathfinder;
    private NavPath _navPath;
    public NavPath NavPath => _navPath;    

    private float _gravity;
    public float Gravity => _gravity;


    private protected bool _isGrounded;
    public bool IsGrounded => _isGrounded;

    public Vector2 Position => transform.position;

    private protected Vector2 _lookDirection;
    private float _jumpTimer;

    public virtual void Init(NavPathfinder pathfinder)
    {
        _rb = GetComponent<Rigidbody2D>();
        _navPathfinder = pathfinder;

        _gravity = -Physics2D.gravity.y * _rb.gravityScale;
    }
    public virtual void Tick(float deltaTime)
    {
        //_isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, Constants.Instance.GroundLayer);
        if (_jumpTimer > 0) _jumpTimer -= deltaTime;
        if (_jumpTimer < 0) _jumpTimer = 0;
        _isGrounded = _jumpTimer == 0 && _groundCheck.IsTouchingLayers(Constants.Instance.GroundLayer);

        _spriteRenderer.flipX = _lookDirection.x < 0;
    }

    public void OnShieldDamaged(int dmg)
    {
        _par_onShieldDMG?.Emit(Constants.Instance.ParticlesOnDMG.Range());
        SpawnWorldText(dmg.ToString().ColorStr(Constants.Instance.Color_shieldDMG));
    }
    public void OnShieldBreak()
    {
        _worldUI.SpawnImage(Constants.Instance.VE_ShieldBreak);
    }
    public void OnHPDamaged(int dmg)
    {
        _par_onHPDMG?.Emit(Constants.Instance.ParticlesOnDMG.Range());
        SpawnWorldText(dmg.ToString().ColorStr(Constants.Instance.Color_DMG));
    }
    public void OnDeath()
    {
        _entityEffectController.transform.parent = null;
        _entityEffectController.DeathEffect();
        Destroy(gameObject);
    }

    public void OnReloadStart(float reloadTime)
    {
        _worldUI.SetSliderUI("Reload");
    }
    public void OnReloading(float currentTime, float reloadTime)
    {
        _worldUI.SetSliderFill(currentTime, reloadTime);
    }
    public void OnReloadCanceled(float reloadTime)
    {
        _worldUI.ResetSliderUI();
    }
    public void OnReloadCompleted(float reloadTime)
    {
        _worldUI.ResetSliderUI();
    }

    private void SpawnWorldText(string str)
    {
        _worldUI.SpawnText(str);
    }


    // -------------------------------------------------------
    // IPathfinder
    // -------------------------------------------------------

    public NavPath FindPath(Vector2 from, Vector2 to, int jumpHeight, int jumpWidth)
    {
        if (_navPathfinder == null)
        {
            DevLog.Warning("[EntityView] NavPathfinderが設定されていません。");
            return NavPath.Unreachable;
        }
        _navPath = _navPathfinder.FindPath(from, to, jumpHeight, jumpWidth);
        return _navPath;
    }

    // -------------------------------------------------------
    // IMover
    // -------------------------------------------------------

    public virtual void Walk(float moveX)
    {
        SetMoveX(moveX);
    }
    public virtual void Jump(Vector2 jump)
    {
        _jumpTimer = Constants.Instance.JumpInterval;
        SetMove(jump);
    }
    public void SetMove(Vector2 move)
    {
        _rb.velocity = move;
    }
    public void SetMoveX(float moveX)
    {
        Vector2 move = new Vector2(moveX, _rb.velocity.y);
        _rb.velocity = move;
    }
    public void SetMoveY(float moveY)
    {
        Vector2 move = new Vector2(_rb.velocity.x, moveY);
        _rb.velocity = move;
    }
    public void AddMoveY(float moveY)
    {
        Vector2 move = _rb.velocity;
        move.y += moveY;
        _rb.velocity = move;
    }

    // -------------------------------------------------------
    // IEntityScanner
    // -------------------------------------------------------

    public IReadOnlyList<EntityModel> Scan(Vector2 toward, float fovAngle, float range, bool ignoreWall)
    {
        // towardの方向、角度fovAngle、半径rangeの扇状にいるエンティティを返す
        Vector2 origin = GetEyeOrigin();
        Vector2 dir = toward - origin;

        List<EntityModel> result = new List<EntityModel>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, Constants.Instance.EntityLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.attachedRigidbody == _rb) continue;// 自分自身は除外

            EntityPresenter presenter = hit.GetComponentInParent<EntityPresenter>();
            EntityModel candidate = presenter?.Model;
            if (candidate == null || !candidate.Alive) continue;

            Vector2 toCandidate = candidate.Position - origin;
            if (dir != Vector2.zero && Vector2.Angle(dir, toCandidate) > fovAngle / 2f) continue;

            if (!ignoreWall && Physics2D.Linecast(origin, candidate.Position, Constants.Instance.ObstacleLayer)) continue;// 障害物に遮られている

            result.Add(candidate);
        }

        return result;
    }

    private Vector2 GetEyeOrigin()
    {
        if (_eyePosition != null) return _eyePosition.position;

        DevLog.Warning("[EntityView] _eyePositionが設定されていません。Positionを代わりに使用します。");
        return Position;
    }
    // -------------------------------------------------------
    // ILooker
    // -------------------------------------------------------
    public virtual void Look(Vector2 lookAt, float angle, float range)
    {
        _lookDirection = new Vector2(
            lookAt.x - transform.position.x,
            lookAt.y - transform.position.y
        );
    }

    // -------------------------------------------------------
    // IProjectileSpawner
    // -------------------------------------------------------

    public void SpawnProjectile(FireParams fireParams)
    {
        float spreadRange = Mathf.Max(fireParams.Snapshot.Spread, 0);
        float spreadDelta = spreadRange / -2f;

        for (int i = 0; i < fireParams.Snapshot.PelletPerShot; i++)
        {
            float spread = 0f;
            if (spreadRange > 0 && !fireParams.Snapshot.Equidistant) { spread = Random.Range(spreadRange / -2f, spreadRange / 2f); }//拡散の決定
            if (fireParams.Snapshot.Equidistant)//等間隔に発射するなら
            {
                spread = spreadDelta;
                spreadDelta += spreadRange / (fireParams.Snapshot.PelletPerShot - 1);
            }

            Vector2 target = fireParams.TargetPos - fireParams.FirePos;

            Quaternion quat;
            if (Vector2.right == -target.normalized) quat = Quaternion.Euler(0, 0, 180f);
            else quat = Quaternion.FromToRotation(Vector3.right, target);


            var p = Instantiate(fireParams.Snapshot.BulletPrefab, fireParams.FirePos, quat);//pjtlの生成
            //p.GetComponent<Bullet>().Init(bulletStatus, this, bulletParams.wpn);
            p.transform.Rotate(new Vector3(0, 0, 1), spread);//拡散分回転させる
            p.GetComponent<Projectile>().Init(fireParams);
        }
    }
}