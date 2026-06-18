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
    }

    //当たった時の効果(要検討)
}

public struct FireParams
{
    private List<Faction> _targetFactions;

    private Vector2 _firePos;
    private Vector2 _targetPos;

    public List<Faction> TargetFaction => _targetFactions;
    public Vector2 FirePos => _firePos;
    public Vector2 TargetPos => _targetPos;

    public PjtlSnapshot Snapshot;

    public FireParams(List<Faction> targetFactions,Vector2 firePos, Vector2 targetPos, PjtlSnapshot snapshot)
    {
        _targetFactions = targetFactions;
        _firePos = firePos;
        _targetPos = targetPos;
        Snapshot = snapshot;
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

    // PjtlStatsの現在値からスナップショットを生成
    public static PjtlSnapshot From(PjtlStats stats)
    {
        return new PjtlSnapshot
        {
            BulletPrefab = stats.BulletPrefab,
            PelletPerShot = stats.PelletPerShot.IntValue,
            Spread = stats.Spread.Value,
            Equidistant = stats.Equidistant,
            BulletSpeed = stats.BulletSpeed.Value,
            BulletLifetime = stats.BulletLifetime.Value,
            BulletSizeMultiplier = stats.BulletSizeMultiplier,
            Penetration = stats.Penetration.IntValue,
        };
    }
}