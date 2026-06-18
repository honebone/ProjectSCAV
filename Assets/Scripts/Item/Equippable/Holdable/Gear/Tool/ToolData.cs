using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具のScriptableObject
/// 持つだけでパッシブ効果、またはUse()でアクティブ効果（消費なし）
/// </summary>
[CreateAssetMenu(menuName = "Item/ToolData")]
public class ToolData : GearData
{

    public override ItemModel CreateModel() { return new ToolModel(this); }
}
