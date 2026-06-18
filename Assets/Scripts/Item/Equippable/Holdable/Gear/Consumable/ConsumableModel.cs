using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消耗品のModel / IUsable
/// Use()で効果を発動し、使用後に消費される
/// </summary>
public class ConsumableModel : GearModel
{
    private readonly ConsumableData _data;

    public ConsumableModel(ConsumableData data) : base(data)
    {
        _data = data;
    }
}
