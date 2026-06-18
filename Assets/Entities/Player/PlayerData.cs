using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/PlayerData")]
public class PlayerData : EntityStatsData
{
    [SerializeField] float _jetpackPower;
    [SerializeField] Vector2Int _inventorySize;

    public Vector2Int InventorySize => _inventorySize;
    public override EntityModel CreateModel(EntityView view)
    {
        if (view is IInputGetter inputGetter) return new PlayerModel(this, _jetpackPower, view, view, inputGetter, view);
        else
        {
            DevLog.Error("[Player Data] view‚ªinputGetter‚ğ‚Á‚Ä‚¢‚Ü‚¹‚ñ");
            return null;
        }
    }
}