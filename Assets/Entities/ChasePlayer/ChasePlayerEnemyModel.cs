using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChasePlayerEnemyModel : EntityModel, IMovable, IEngagable
{
    private EntityModel _target;
    public EntityModel Target => _target;
    public bool Engaged => _target != null;

    private Vector2 _lookAt;
    public Vector2 LookAt => _lookAt;

    private float _jumpTimer;
    //private bool _jumpFlag;

    private bool _isJumping;
    public bool IsJumping => _isJumping;

    private readonly IEntityScanner _entityScanner;
    private readonly IPathfinder _pathfinder;
    private readonly IMover _mover;

    // 経路
    private NavPath _currentPath;
    private int _pathIndex;
    private Vector2 _jumpVector;
    private float _repathTimer;

    // 再計算判定用：前回の目標ワールド座標
    private Vector2 _lastTargetPos;

    // 再計算をトリガーする距離の二乗（ルートを避けるため二乗で比較）
    private const float RepathThresholdSq = 1f; // 1マス相当
    // ノードに到着したとみなす距離
    private const float ArrivedRange = 0.1f; // 1マス相当

    public ChasePlayerEnemyModel(
        EntityStatsData data,
        IEntityScanner entityScanner,
        IProjectileSpawner projectileSpawner,
        IPathfinder pathfinder,
        IMover mover) : base(data, projectileSpawner)
    {
        _entityScanner = entityScanner;
        _pathfinder = pathfinder;
        _mover = mover;
    }

    public override void Tick(float deltaTime, Vector2 position)
    {
        base.Tick(deltaTime, position);

        if (_jumpTimer > 0) _jumpTimer -= deltaTime;
        if (_repathTimer > 0) _repathTimer -= deltaTime;
        if (_isJumping && _mover.IsGrounded && _jumpTimer <= 0) _isJumping = false;

        if (_isJumping) Walk(_jumpVector.x);//ジャンプ中は水平方向のみ速度を設定(壁にこすったとき用)
        else
        {
            if (Engaged)
            {
                Vector2 targetPos = _target.Position;

                //// 目標が一定距離以上移動していたら経路を再計算
                //if (NeedsRepath(targetPos))
                //{
                //    FindPath(targetPos);
                //    DevLog.Log("Repath");
                //}
                if (_repathTimer < 0)
                {
                    FindPath(targetPos);
                }

                if (CheckSameGroundAsTarget())
                {
                    WalkToward(Target.Position);
                }
                else
                {
                    if (CheckArrived(GetTargetEdgeType()))
                    {
                        _pathIndex++;
                    }

                    // 経路に沿って移動
                    FollowPath(position, deltaTime);
                }

                // 視界内に交戦対象がいなくなったら離脱
            }
            else
            {
                // 視界内に敵対勢力がいたらEngage
            }
        }
    }

    private void FindPath(Vector2 targetPos)
    {
        _currentPath = _pathfinder.FindPath(
            Position,
            targetPos,
            Stats.JumpHeight.Value.ToInt(),
            Stats.JumpWidth.Value.ToInt());
        _pathIndex = 0;

        _lastTargetPos = targetPos;
        _repathTimer = 1f;
    }

    private bool NeedsRepath(Vector2 targetPos)
    {
        if (_currentPath == null || !_currentPath.IsReachable) return false;
        if (_pathIndex >= _currentPath.Nodes.Count) return false;

        float dx = targetPos.x - _lastTargetPos.x;
        float dy = targetPos.y - _lastTargetPos.y;
        return dx * dx + dy * dy >= RepathThresholdSq;
    }

    private bool CheckArrived(EdgeType edgeType)
    {
        if(_currentPath == null) return false;
        if (!_mover.IsGrounded) return false;

        NavNode targetNode = _currentPath.Nodes[_pathIndex];
        if (edgeType == EdgeType.Jump)
        {
            return _pathfinder.NavPathfinder.IsOnSameGround(Position, targetNode.WorldPos);
        }
        else
        {
            //float dist = Mathf.Abs(Position.x - targetNode.WorldPos.x);
            float dist = Vector2.Distance(Position, targetNode.WorldPos);
            return dist <= ArrivedRange;
        }
    }

    private bool CheckSameGroundAsTarget()
    {
        if (_currentPath != null && _currentPath.IsSameGround) return true;
        if (_pathIndex >= _currentPath.Nodes.Count) return true;

        return false;
    }

    private void FollowPath(Vector2 position, float deltaTime)
    {
        if (_currentPath == null || !_currentPath.IsReachable) return;
        if (!_currentPath.IsSameGround && !_currentPath.IsReachable) return;//到達不可能の場合
        if (_pathIndex >= _currentPath.Nodes.Count) return;

        NavNode targetNode = _currentPath.Nodes[_pathIndex];

        EdgeType edgeType = GetTargetEdgeType();
        Vector2 targetPos = targetNode.WorldPos;

        if (edgeType != EdgeType.Jump)
        {
            WalkToward(targetPos);
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

    private void WalkToward(Vector2 targetPos)
    {
        int walkDir = !_mover.IsGrounded ? 0 : Position.x > targetPos.x ? -1 : 1;
        Walk(walkDir * Stats.MoveSpeed.Value);
    }

    // -------------------------------------------------------
    // IMovable
    // -------------------------------------------------------

    public void Walk(float move) { _mover.SetMoveX(move); }
    public void Jump(Vector2 jump)
    {
        if (!_isJumping)
        {
            _isJumping = true;
            _jumpTimer = 0.1f;
            _mover.SetMove(jump);
        }
    }

    //private bool ReachedNode( NavNode targetNode, bool onlyX = false)
    //{
    //    Vector2 targetPos = new Vector2(targetNode.Cell.x, targetNode.Cell.y);
    //}

    // -------------------------------------------------------
    // IEngagable
    // -------------------------------------------------------

    public void Engage(EntityModel target)
    {
        _target = target;
        FindPath(_target.Position);
    }
    public void Disengage() { _target = null; }
}