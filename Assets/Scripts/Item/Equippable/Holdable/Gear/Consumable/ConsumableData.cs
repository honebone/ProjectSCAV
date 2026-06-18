using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Á–Õ•i‚ÌScriptableObject
/// g—p‚ÉÁ”ï‚³‚ê‚é
/// </summary>
[CreateAssetMenu(menuName = "Item/ConsumableData")]
public class ConsumableData : GearData
{
    public override ItemModel CreateModel() { return new ConsumableModel(this); }
}
