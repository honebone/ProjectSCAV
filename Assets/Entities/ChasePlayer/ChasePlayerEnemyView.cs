using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChasePlayerEnemyModel;

public class ChasePlayerEnemyView : EntityView, IItemVisualizer
{
    [SerializeField] private HoldingItemView _holdingItemView;
    [SerializeField] private Animator _engageLampAnim;
    public HoldingItemView HoldingItemView => _holdingItemView;

    public override void Init(NavPathfinder pathfinder)
    {
        base.Init(pathfinder);

        _holdingItemView.Init(this);
    }

    public void OnStateChanged(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Engage:
                _entityEffectController.SpawnEffectObject(Constants.Instance.VE_Engage);
                _engageLampAnim.SetTrigger("Engage");
                break;
            case EnemyState.Alert:
                _entityEffectController.SpawnEffectObject(Constants.Instance.VE_LostSight);
                _engageLampAnim.SetTrigger("Alert");
                break;
            default: _engageLampAnim.SetTrigger("Normal"); break;
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
