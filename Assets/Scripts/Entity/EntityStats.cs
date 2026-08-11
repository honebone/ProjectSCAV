using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats
{
    public StatValue MaxHp { get; }
    public StatValue MaxArmor { get; }
    public StatValue ArmorRegenDelay { get; }
    public StatValue ArmorRegenSpeed { get; }

    public StatValue MoveSpeed { get; }
    // JumpPowerはVector2のため個別に管理
    public StatValue JumpHeight { get; }
    public StatValue JumpWidth { get; }

    public StatValue FOVAngle { get; }
    public StatValue SightRange { get; }

    // --- Heat（プレイヤー専用。敵には不要だが基底クラスに定義しておく）---
    public StatValue MaxHeat { get; }
    public StatValue HeatRecovery { get; }
    public StatValue JetpackPower { get; }

    public EntityStats(EntityStatsData data)
    {
        MaxHp = new ClampedStatValue(data.MaxHp, minValue: 1);
        MaxArmor = new ClampedStatValue(data.MaxArmor);
        ArmorRegenDelay = new ClampedStatValue(data.ArmorRegenDelay);
        ArmorRegenSpeed = new ClampedStatValue(data.ArmorRegenSpeed);

        MoveSpeed = new ClampedStatValue(data.MoveSpeed);
        JumpHeight = new ClampedStatValue(data.JumpPower.y);
        JumpWidth = new ClampedStatValue(data.JumpPower.x);

        FOVAngle = new ClampedStatValue(data.FOVAngle);
        SightRange = new ClampedStatValue(data.SightRange);

        MaxHeat = new ClampedStatValue(data.MaxHeat);
        HeatRecovery = new ClampedStatValue(data.HeatRecovery);
        JetpackPower = new ClampedStatValue(data.JetpackPower);
    }

    /// <summary>種別を指定して対象のStatValueを取得する。バフ・パッシブ効果からの補正はこの窓口経由で行う</summary>
    public StatValue Get(EntityStatType type)
    {
        switch (type)
        {
            case EntityStatType.MoveSpeed: return MoveSpeed;
            case EntityStatType.MaxHp: return MaxHp;
            case EntityStatType.MaxArmor: return MaxArmor;
            case EntityStatType.ArmorRegenDelay: return ArmorRegenDelay;
            case EntityStatType.ArmorRegenSpeed: return ArmorRegenSpeed;
            case EntityStatType.JumpHeight: return JumpHeight;
            case EntityStatType.JumpWidth: return JumpWidth;
            case EntityStatType.FOVAngle: return FOVAngle;
            case EntityStatType.SightRange: return SightRange;
            case EntityStatType.MaxHeat: return MaxHeat;
            case EntityStatType.HeatRecovery: return HeatRecovery;
            case EntityStatType.JetpackPower: return JetpackPower;
            default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

/// <summary>EntityStatsが保持するステータスの種別。バフ・パッシブ効果からの補正先を指定するキーとして使う</summary>
public enum EntityStatType
{
    MoveSpeed, MaxHp, MaxArmor, ArmorRegenDelay, ArmorRegenSpeed,
    JumpHeight, JumpWidth, FOVAngle, SightRange, MaxHeat, HeatRecovery,JetpackPower
}
