using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;

public class EntityView : MonoBehaviour, IEntityScanner, IProjectileSpawner, IPathfinder,IMover,ILooker
{
    [Header("Ground Detection")]
    [SerializeField] private Collider2D _groundCheck;      // 足元のオブジェクト
    [SerializeField] private LayerMask _groundLayer;      // 地面とするレイヤー
    [SerializeField] private ParticleSystem _par_onArmorDMG;
    [SerializeField] private ParticleSystem _par_onHPDMG;

    private Rigidbody2D _rb;
    private NavPathfinder _navPathfinder;
    public NavPathfinder NavPathfinder => _navPathfinder;
    private NavPath _navPath;
    public NavPath NavPath => _navPath;

    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;

    private float _gravity;
    public float Gravity => _gravity;

    public Vector2 Position => transform.position;

    public virtual void Init(NavPathfinder pathfinder)
    {
        _rb = GetComponent<Rigidbody2D>();
        _navPathfinder = pathfinder;

        _gravity = -Physics2D.gravity.y * _rb.gravityScale;
    }
    public virtual void Tick(float deltaTime)
    {
        //_isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);
        _isGrounded = _groundCheck.IsTouchingLayers(_groundLayer);
    }

    public void OnArmorDamaged(int dmg)
    {
        _par_onArmorDMG.Emit(Constants.Instance.ParticlesOnDMG.Range());
    }
    public void OnHPDamaged(int dmg)
    {
        _par_onHPDMG.Emit(Constants.Instance.ParticlesOnDMG.Range());
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
        return null;
    }
    // -------------------------------------------------------
    // ILooker
    // -------------------------------------------------------
    public virtual void Look(Vector2 lookAt, float angle, float range) { }

    // -------------------------------------------------------
    // IProjectileSpawner
    // -------------------------------------------------------

    public void SpawnProjectile(FireParams fireParams)
    {
        Quaternion quat;
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
            if (Vector2.right == -target.normalized) quat = Quaternion.Euler(0, 0, 180f);
            else quat = Quaternion.FromToRotation(Vector3.right, target);


            var p = Instantiate(fireParams.Snapshot.BulletPrefab, fireParams.FirePos, quat);//pjtlの生成
            //p.GetComponent<Bullet>().Init(bulletStatus, this, bulletParams.wpn);
            p.transform.Rotate(new Vector3(0, 0, 1), spread);//拡散分回転させる
            p.GetComponent<Projectile>().Init(fireParams);
        }
    }
}