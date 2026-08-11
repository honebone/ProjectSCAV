using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手持ち装備系アイテムの基底Model
/// </summary>
public abstract class GearModel : HoldableItemModel
{
    protected GearModel(GearData data) : base(data) { }
}
