using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 巡回・交戦・警戒・帰還の4状態を遷移する追跡型の敵Model
/// 状態遷移の仕様は 敵AIの状態遷移.md を参照
/// </summary>
public class ChasePlayerEnemyModel : EntityModel, IMovable, IEngagable, ILookable, ILoadoutable
{
    public enum EnemyState { Patrol, Engage, Alert, Return }

    private EnemyState _state = EnemyState.Patrol;
    public EnemyState State => _state;

    /// <summary>状態が変化したときに通知する（View側の演出等が将来購読する想定）</summary>
    public event Action<EnemyState> OnStateChanged;

    private EntityModel _target;
    public EntityModel Target => _target;
    public bool Engaged => _target != null;

    private Vector2 _lookAt;
    public Vector2 LookAt => _lookAt;

    private float _jumpTimer;
    private bool _isJumping;
    public bool IsJumping => _isJumping;

    private bool _isWalking;
    public bool IsMoving => _isWalking || !_mover.IsGrounded;

    private readonly ChasePlayerEnemyData _data;
    private readonly IEntityScanner _entityScanner;
    private readonly IPathfinder _pathfinder;
    private readonly IMover _mover;
    private readonly ILooker _looker;

    private readonly LoadoutModel _loadout;
    public LoadoutModel Loadout => _loadout;

    // 経路
    private NavPath _currentPath;
    private int _pathIndex;
    private Vector2 _jumpVector;
    private float _repathTimer;

    // ノードに到着したとみなす距離
    private const float ArrivedRange = 0.1f;

    // 持ち場
    private Vector2 _home;
    private bool _homeSet;

    // 巡回
    private Vector2 _patrolTarget;
    private bool _hasPatrolTarget;
    private float _patrolPauseTimer;

    // 交戦
    private float _freezeOnEngageTimer;
    private float _loseSightTimer;
    private bool _isFiring;

    // 警戒
    private Vector2 _alertDestination;
    private bool _alertArrived;
    private float _alertLookTimer;

    public ChasePlayerEnemyModel(
        ChasePlayerEnemyData data,
        IEntityScanner entityScanner,
        IProjectileSpawner projectileSpawner,
        IPathfinder pathfinder,
        IMover mover,
        ILooker looker) : base(data, projectileSpawner)
    {
        _data = data;
        _entityScanner = entityScanner;
        _pathfinder = pathfinder;
        _mover = mover;
        _looker = looker;

        _loadout = new LoadoutModel(data.GunSlot, data.GearSlot, data.ImplantSlot, this);
        if (data.StartingGun != null)
        {
            ItemStackModel gunStack = new ItemStackModel(data.StartingGun.CreateModel(), 1);
            _loadout.TryEquip(0, gunStack, this);
        }
    }

    public override void Tick(float deltaTime, Vector2 position)
    {
        base.Tick(deltaTime, position);

        if (!_homeSet)
        {
            _home = position;
            _homeSet = true;
        }

        _loadout.Tick(deltaTime, this);

        if (_jumpTimer > 0) _jumpTimer -= deltaTime;
        if (_repathTimer > 0) _repathTimer -= deltaTime;
        if (_isJumping && _mover.IsGrounded && _jumpTimer <= 0) _isJumping = false;

        if (_isJumping)
        {
            Walk(_jumpVector.x);//ジャンプ中は水平方向のみ速度を設定(壁にこすったとき用)
            return;
        }

        if (_state != EnemyState.Engage)//敵を視認した場合は交戦状態に入る
        {
            EntityModel hostile = FindVisibleHostile();
            if (hostile != null)
            {
                Engage(hostile);
            }
        }

        switch (_state)
        {
            case EnemyState.Patrol: TickPatrol(deltaTime, position); break;
            case EnemyState.Engage: TickEngage(deltaTime, position); break;
            case EnemyState.Alert: TickAlert(deltaTime, position); break;
            case EnemyState.Return: TickReturn(deltaTime, position); break;
        }       
    }

    // -------------------------------------------------------
    // 巡回
    // -------------------------------------------------------

    private void TickPatrol(float deltaTime, Vector2 position)
    {
        if (_patrolPauseTimer > 0)
        {
            _patrolPauseTimer -= deltaTime;
            return;
        }

        if (!_hasPatrolTarget) PickNewPatrolTarget();

        //_lookAt = _patrolTarget;
        //Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        if (HasArrived(_patrolTarget, position))
        {
            Stop();
            _patrolPauseTimer = _data.PatrolPauseTime.Range();
            _hasPatrolTarget = false;
        }
        else MoveToward(_patrolTarget, position, deltaTime, _data.PatrolWalkSpeed);
        //bool arrived = MoveToward(_patrolTarget, position, deltaTime);
        //if (arrived)
        //{
        //    _patrolPauseTimer = _data.PatrolPauseTime.Range();
        //    _hasPatrolTarget = false;
        //}
    }

    // 巡回目標地点の探索リトライ回数（到達可能な地点が見つかるまで試行する）
    private const int PatrolTargetMaxAttempts = 8;

    private void PickNewPatrolTarget()
    {
        for (int i = 0; i < PatrolTargetMaxAttempts; i++)
        {
            Vector2 candidate = _home + UnityEngine.Random.insideUnitCircle * _data.PatrolRadius;
            FindPath(candidate);// _currentPathを実際に更新し、到達可否を判定する

            if (_currentPath.IsReachable || _currentPath.IsSameGround)
            {
                DevLog.Log($"巡回地点：{candidate}");
                _patrolTarget = candidate;
                _hasPatrolTarget = true;
                return;
            }
        }

        // 半径内に到達可能な地点が見つからなければ持ち場へフォールバック（必ず到達可能）
        _patrolTarget = _home;
        _hasPatrolTarget = true;
        FindPath(_home);
    }

    // -------------------------------------------------------
    // 交戦
    // -------------------------------------------------------

    private void TickEngage(float deltaTime, Vector2 position)
    {
        if(_freezeOnEngageTimer > 0)
        {
            _freezeOnEngageTimer -= deltaTime;
            return;
        }

        if (_target == null || !_target.Alive)
        {
            Disengage();
            EnterReturn();
            return;
        }

        _lookAt = _target.Position;
        Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        bool visible = IsTargetVisible();
        if (visible)
        {
            _loseSightTimer = _data.LoseSightAlertDelay;
        }
        else
        {
            _loseSightTimer -= deltaTime;
            if (_loseSightTimer <= 0)
            {
                Vector2 lastKnownPos = _target.Position;
                Disengage();
                EnterAlert(lastKnownPos);
                return;
            }
        }

        bool stopToFight = CheckSameGroundAsTarget() && Vector2.Distance(position, _target.Position) <= _data.EngageStopDistance;
        if (!stopToFight)
        {
            MoveToward(_target.Position, position, deltaTime, lookMovingDir: false);
        }
        else Stop();
        bool inRange = Vector2.Distance(position, _target.Position) <= Stats.SightRange.Value;
        SetFiring(visible && inRange);
    }

    private bool IsTargetVisible()
    {
        if (_target == null) return false;
        IReadOnlyList<EntityModel> visible = _entityScanner.Scan(_target.Position, Stats.FOVAngle.Value, Stats.SightRange.Value, false);
        return visible != null && visible.Contains(_target);
    }

    private EntityModel FindVisibleHostile()
    {
        IReadOnlyList<EntityModel> visible = _entityScanner.Scan(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value, false);
        if (visible == null) return null;

        foreach (EntityModel candidate in visible)
        {
            if (Hostiles.Contains(candidate.Faction)) return candidate;
        }
        return null;
    }

    private void SetFiring(bool shouldFire)
    {
        if (shouldFire == _isFiring) return;

        if (shouldFire) _loadout.HoldingItem?.Use(this);
        else _loadout.HoldingItem?.StopUsing(this);

        _isFiring = shouldFire;
    }

    // -------------------------------------------------------
    // 警戒
    // -------------------------------------------------------

    private void TickAlert(float deltaTime, Vector2 position)
    {
        //if (!_alertArrived)
        //{
        //    _lookAt = _alertDestination;
        //    Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        //    if (MoveToward(_alertDestination, position, deltaTime)) _alertArrived = true;
        //    return;
        //}

        if (!HasArrived(_alertDestination,position))
        {
            //_lookAt = _alertDestination;
            //Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

            MoveToward(_alertDestination, position, deltaTime);
            return;
        }
        Stop();

        _alertLookTimer += deltaTime;
        float t = Mathf.Clamp01(_alertLookTimer / Mathf.Max(_data.AlertLookDuration, 0.01f));
        float angleRad = Mathf.Lerp(-180f, 180f, t) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        _lookAt = position + dir * Stats.SightRange.Value;
        Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        if (_alertLookTimer >= _data.AlertLookDuration) EnterReturn();
    }

    // -------------------------------------------------------
    // 帰還
    // -------------------------------------------------------

    private void TickReturn(float deltaTime, Vector2 position)
    {
        //_lookAt = _home;
        //Look(_lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        if (MoveToward(_home, position, deltaTime)) EnterPatrol();
    }

    // -------------------------------------------------------
    // 状態遷移
    // -------------------------------------------------------

    private void EnterPatrol()
    {
        Stop();
        _state = EnemyState.Patrol;
        _hasPatrolTarget = false;
        _patrolPauseTimer = 0;
        OnStateChanged?.Invoke(_state);
    }

    private void EnterAlert(Vector2 lastKnownPos)
    {
        Stop();
        _state = EnemyState.Alert;
        _alertDestination = lastKnownPos;
        _alertArrived = false;
        _alertLookTimer = 0;
        OnStateChanged?.Invoke(_state);
    }

    private void EnterReturn()
    {
        Stop();
        _state = EnemyState.Return;
        OnStateChanged?.Invoke(_state);
    }

    // -------------------------------------------------------
    // 経路追従（巡回・交戦・帰還で共通利用）
    // -------------------------------------------------------

    private bool HasArrived(Vector2 destination, Vector2 position) => _mover.IsGrounded && Vector2.Distance(position, _pathfinder.NavPathfinder.ResolveGroundPos(destination)) <= ArrivedRange;
    /// <summary>
    /// 指定した目的地に向かって経路探索・追従を行う。目的地に十分近づいたらtrueを返す
    /// </summary>
    private bool MoveToward(Vector2 destination, Vector2 position, float deltaTime, float walkSpeedRatio = 1f, bool lookMovingDir = true)
    {
        if (_currentPath == null || _repathTimer < 0)
        {
            FindPath(destination);
        }

        if (CheckSameGroundAsTarget())
        {
            WalkToward(destination, walkSpeedRatio, lookMovingDir);
        }
        else
        {
            if (CheckArrived(GetTargetEdgeType()))
            {
                _pathIndex++;
            }
            FollowPath(position, deltaTime, walkSpeedRatio, lookMovingDir);
        }

        return HasArrived(destination, position);
    }

    private void FindPath(Vector2 targetPos)
    {
        _currentPath = _pathfinder.FindPath(
            Position,
            targetPos,
            Stats.JumpHeight.Value.ToInt(),
            Stats.JumpWidth.Value.ToInt());
        _pathIndex = 0;
        _repathTimer = 1f;
    }

    private bool CheckArrived(EdgeType edgeType)
    {
        if (_currentPath == null) return false;
        if (!_mover.IsGrounded) return false;

        NavNode targetNode = _currentPath.Nodes[_pathIndex];
        if (edgeType == EdgeType.Jump)
        {
            return _pathfinder.NavPathfinder.IsOnSameGround(Position, targetNode.WorldPos);
        }
        else
        {
            float dist = Vector2.Distance(Position, targetNode.WorldPos);
            return dist <= ArrivedRange;
        }
    }

    private bool CheckSameGroundAsTarget()
    {
        if (_currentPath == null) return false;
        if (_currentPath.IsSameGround) return true;
        if (_pathIndex >= _currentPath.Nodes.Count) return true;

        return false;
    }

    private void FollowPath(Vector2 position, float deltaTime, float walkSpeedRatio = 1f, bool lookMovingDir = true)
    {
        if (_currentPath == null || !_currentPath.IsReachable) return;
        if (!_currentPath.IsSameGround && !_currentPath.IsReachable) return;//到達不可能の場合
        if (_pathIndex >= _currentPath.Nodes.Count) return;

        NavNode targetNode = _currentPath.Nodes[_pathIndex];

        EdgeType edgeType = GetTargetEdgeType();
        Vector2 targetPos = targetNode.WorldPos;

        if (edgeType != EdgeType.Jump)
        {
            WalkToward(targetPos, walkSpeedRatio, lookMovingDir);
        }
        else
        {
            if (!_isJumping)
            {
                // 距離の差分
                float diffX = targetPos.x - Position.x;
                float diffY = targetPos.y - Position.y + 1;

                float jumpDuration = 0.5f;
                // 1. 水平方向の速度 v_x = 距離 / 時間
                float vx = diffX / jumpDuration;

                // 2. 垂直方向の速度 v_y = (y + 0.5 * g * t^2) / t
                float vy = (diffY + 0.5f * _mover.Gravity * Mathf.Pow(jumpDuration, 2)) / jumpDuration;
                _jumpVector = new Vector2(vx, vy);

                if (lookMovingDir)
                {
                    Vector2 lookAt = position + new Vector2(Mathf.Sign(diffX) * 3f, 0);
                    Look(lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);
                }
                Jump(_jumpVector);
            }
            else Walk(_jumpVector.x);//ジャンプ中は水平方向のみ速度を設定(壁にこすったとき用)
        }
    }

    private EdgeType GetTargetEdgeType()
    {
        NavNode targetNode = _currentPath.Nodes[_pathIndex];
        //最初のノードへは歩いていく それ以降は、一つ前のノード(出発地点)から目標ノードまでのエッジを取得する
        return _pathIndex == 0 ? EdgeType.Walk : _currentPath.Nodes[_pathIndex - 1].GetEdge(targetNode).Type;
    }

    private void WalkToward(Vector2 targetPos, float speedRatio = 1f, bool lookMovingDir = true)
    {
        int walkDir = !_mover.IsGrounded ? 0 : Position.x > targetPos.x ? -1 : 1;

        if (lookMovingDir)
        {
            Vector2 lookAt = Position + new Vector2(Mathf.Sign(walkDir) * 3f, 0);
            Look(lookAt, Stats.FOVAngle.Value, Stats.SightRange.Value);
        }
        Walk(walkDir * Stats.MoveSpeed.Value * speedRatio);
    }

    // -------------------------------------------------------
    // IMovable
    // -------------------------------------------------------
    private void Stop()
    {
        _mover.SetMoveX(0);
    }
    public void Walk(float move)
    {
        _isWalking = move != 0;
        _mover.Walk(move);
    }

    public void Jump(Vector2 jump)
    {
        if (!_isJumping)
        {
            _isJumping = true;
            _jumpTimer = 0.1f;
            _mover.Jump(jump);
        }
    }

    // -------------------------------------------------------
    // ILookable
    // -------------------------------------------------------

    public void Look(Vector2 lookAt, float angle, float range)
    {
        _lookAt = lookAt;
        _looker.Look(lookAt, angle, range);
    }

    // -------------------------------------------------------
    // IEngagable
    // -------------------------------------------------------

    public void Engage(EntityModel target)
    {
        Stop();
        SetFiring(false);
        _target = target;
        _state = EnemyState.Engage;
        _freezeOnEngageTimer = _data.FreezeTimeOnEngage;
        _loseSightTimer = _data.LoseSightAlertDelay;
        FindPath(_target.Position);
        OnStateChanged?.Invoke(_state);
    }

    public void Disengage()
    {
        SetFiring(false);
        _target = null;
    }
}
