using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChasePlayerEnemyModel;

public class ChasePlayerEnemyView : EntityView, IItemVisualizer
{
    [SerializeField] private HoldingItemView _holdingItemView;
    public HoldingItemView HoldingItemView => _holdingItemView;

    public override void Init(NavPathfinder pathfinder)
    {
        base.Init(pathfinder);

        _holdingItemView.init(this);
    }

    public void OnStateChanged(EnemyState state)
    {
        DevLog.Log($"ステート遷移:{state}");
        switch (state)
        {
            case EnemyState.Engage: _entityEffectController.SpawnEffectObject(Constants.Instance.VE_Engage); break;
            case EnemyState.Alert: _entityEffectController.SpawnEffectObject(Constants.Instance.VE_LostSight); break;
        }
    }

    public override void Look(Vector2 lookAt, float angle, float range)
    {
        base.Look(lookAt, angle, range);

        _holdingItemView.UpdateAim(lookAt);
    }

    public void OnItemHeld(HoldableItemModel model)
    {
        _holdingItemView.OnItemHeld(model);
    }
    public void UpdateAim(Vector2 lookAt)
    {
        _holdingItemView.UpdateAim(lookAt);
    }
}
