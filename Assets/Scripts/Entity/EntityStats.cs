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
    }
}
