using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileSpawner
{
    void SpawnProjectile(FireParams fireParams);
}

public class PjtlStats
{
    public GameObject BulletPrefab { get; }

    public StatValue PelletPerShot { get; }//一回の生成で何発飛ばすか

    public StatValue Spread { get; }//拡散角度 +- spread/2度ばらつく
    public bool Equidistant { get; }//pelletPerFireが2以上のとき、spread角で等間隔に発射する

    public StatValue BulletSpeed { get; }
    public StatValue BulletLifetime { get; }
    public float BulletSizeMultiplier { get; }
    public StatValue Penetration { get; }
    public StatValue Damage { get; }
    /// <summary>命中時に対象へ付与するデバフ（酸弾等）。パッシブ補正の対象ではなく弾種固有のデータとしてそのまま転送する</summary>
    public IReadOnlyList<BuffApplication> OnHitBuffs { get; }

    public PjtlStats(PjtlData data)
    {
        BulletPrefab = data.BulletPrefab;
        PelletPerShot = new ClampedStatValue(data.PelletPerShot, 1);
        Spread = new ClampedStatValue(data.Spread);
        Equidistant = data.Equidistant;
        BulletSpeed = new ClampedStatValue(data.BulletSpeed);
        BulletLifetime = new ClampedStatValue(data.BulletLifetime);
        BulletSizeMultiplier = 1f;
        Penetration = new ClampedStatValue(data.Penetration);
        Damage = new ClampedStatValue(data.Damage);
        OnHitBuffs = data.OnHitBuffs;
    }
}

public struct FireParams
{
    private List<Faction> _targetFactions;

    private Vector2 _firePos;
    private Vector2 _targetPos;
    private EntityModel _source;

    public List<Faction> TargetFaction => _targetFactions;
    public Vector2 FirePos => _firePos;
    public Vector2 TargetPos => _targetPos;
    /// <summary>発射元のエンティティ。命中時のEffectActionの発生源として使う</summary>
    public EntityModel Source => _source;

    public PjtlSnapshot Snapshot;

    public FireParams(List<Faction> targetFactions,Vector2 firePos, Vector2 targetPos, PjtlSnapshot snapshot, EntityModel source)
    {
        _targetFactions = targetFactions;
        _firePos = firePos;
        _targetPos = targetPos;
        Snapshot = snapshot;
        _source = source;
    }

    public void SetFirePos(Vector2 pos) { _firePos = pos; }
}

// 射撃1回分の投射物パラメータのスナップショット（値型）
public struct PjtlSnapshot
{
    public GameObject BulletPrefab;
    public int PelletPerShot;
    public float Spread;
    public bool Equidistant;
    public float BulletSpeed;
    public float BulletLifetime;
    public float BulletSizeMultiplier;
    public int Penetration;
    public float Damage;
    /// <summary>命中時に対象へ付与するデバフ（酸弾等）</summary>
    public IReadOnlyList<BuffApplication> OnHitBuffs;

    // PjtlStatsの現在値からスナップショットを生成する
    // mods（発射者の装備中パッシブによる補正）を渡すと、Pull型で都度補正を合成する
    public static PjtlSnapshot From(PjtlStats stats, PassiveModifierSet mods = null)
    {
        return new PjtlSnapshot
        {
            BulletPrefab = stats.BulletPrefab,
            PelletPerShot = stats.PelletPerShot.IntValue,
            Spread = stats.Spread.Value,
            Equidistant = stats.Equidistant,
            BulletSpeed = mods != null ? mods.ApplyPjtl(PjtlStatType.BulletSpeed, stats.BulletSpeed.Value) : stats.BulletSpeed.Value,
            BulletLifetime = mods != null ? mods.ApplyPjtl(PjtlStatType.BulletLifetime, stats.BulletLifetime.Value) : stats.BulletLifetime.Value,
            BulletSizeMultiplier = stats.BulletSizeMultiplier,
            Penetration = mods != null ? Mathf.RoundToInt(mods.ApplyPjtl(PjtlStatType.Penetration, stats.Penetration.Value)) : stats.Penetration.IntValue,
            Damage = mods != null ? mods.ApplyPjtl(PjtlStatType.Damage, stats.Damage.Value) : stats.Damage.Value,
            OnHitBuffs = stats.OnHitBuffs,
        };
    }
}
