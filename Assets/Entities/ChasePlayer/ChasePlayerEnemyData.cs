using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/ChasePlayerEnemyData")]
public class ChasePlayerEnemyData : EntityStatsData
{
    [Header("Loadout")]
    [SerializeField] private GunData _startingGun;

    [Header("巡回")]
    [SerializeField] private float _patrolRadius = 5f;
    [SerializeField] private Vector2 _patrolPauseTime = new Vector2(1f, 3f);
    [SerializeField] private float _patrolWalkSpeed = 0.5f;

    [Header("交戦")]
    [SerializeField] private float _freezeTimeOnEngage;
    [SerializeField] private float _engageStopDistance = 3f;
    [SerializeField] private float _loseSightAlertDelay = 2f;

    [Header("警戒")]
    [SerializeField] private float _alertLookDuration = 3f;

    public GunData StartingGun => _startingGun;
    public float PatrolRadius => _patrolRadius;
    public Vector2 PatrolPauseTime => _patrolPauseTime;
    public float PatrolWalkSpeed => _patrolWalkSpeed;
    public float FreezeTimeOnEngage => _freezeTimeOnEngage;
    public float EngageStopDistance => _engageStopDistance;
    public float LoseSightAlertDelay => _loseSightAlertDelay;
    public float AlertLookDuration => _alertLookDuration;

    public override EntityModel CreateModel(EntityView view)
    {
        return new ChasePlayerEnemyModel(this, view, view, view, view, view);
    }
}
