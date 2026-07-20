using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStats
{
    public StatValue MagCap { get; }
    public StatValue ReloadTime { get; }
    public StatValue FireRate { get; }
    /// <summary>射撃時の拡散ペナルティ 一発撃つごとに追加</summary>
    public StatValue SpreadPenalty_Move { get; }
    /// <summary>移動中の拡散ペナルティ</summary>
    public StatValue SpreadPenalty_Fire { get; }
    public StatValue MaxSpreadPenalty_fire { get; }
    /// <summary>射撃時の拡散ペナルティ減少量 [度/s]</summary>
    public StatValue SpreadLoss { get; }
    public PjtlStats PjtlStats { get; }


    public GunStats(GunData data)
    {
        MagCap = new ClampedStatValue(data.MagCap, minValue: 1);
        ReloadTime = new ClampedStatValue(data.ReloadTime);
        FireRate = new ClampedStatValue(data.FireRate);
        MaxSpreadPenalty_fire = new ClampedStatValue(data.MaxSpreadPenalty_fire);
        SpreadPenalty_Move = new ClampedStatValue(data.SpreadPenalty_Move);
        SpreadPenalty_Fire = new ClampedStatValue(data.SpreadPenalty_Fire);
        SpreadLoss = new ClampedStatValue(data.SpreadLoss);
        PjtlStats = new PjtlStats(data.PjtlData);
    }
}
