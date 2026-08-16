/// <summary>
/// 投射物が命中した瞬間の状態を表す読み取り専用データ
/// IProjectileHitModifier.Modify()へ渡され、命中相手や弾自身の状態に応じたEffectActionの動的補正に使う
/// </summary>
public readonly struct ProjectileHitContext
{
    /// <summary>命中した対象</summary>
    public EntityModel Target { get; }
    /// <summary>発射元（環境要因等はnullを許容する）</summary>
    public EntityModel Source { get; }
    /// <summary>発射地点から命中地点までの距離</summary>
    public float DistanceTraveled { get; }
    /// <summary>発射されてから命中するまでに経過した時間</summary>
    public float TimeAlive { get; }
    /// <summary>貫通で何体目の命中か（0始まり）</summary>
    public int HitIndex { get; }
    /// <summary>この弾の発射時スナップショット</summary>
    public PjtlSnapshot Snapshot { get; }

    public ProjectileHitContext(EntityModel target, EntityModel source, float distanceTraveled, float timeAlive, int hitIndex, PjtlSnapshot snapshot)
    {
        Target = target;
        Source = source;
        DistanceTraveled = distanceTraveled;
        TimeAlive = timeAlive;
        HitIndex = hitIndex;
        Snapshot = snapshot;
    }
}
