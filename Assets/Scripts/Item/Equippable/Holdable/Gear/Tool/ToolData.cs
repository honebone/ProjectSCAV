using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具のScriptableObject
/// 持つだけでパッシブ効果、またはUse()でアクティブ効果（両方でも可）を持つ
/// ステータス補正だけで表現できる道具は、StatModifiersを設定するだけでよい（C#クラスを分ける必要はない）
/// </summary>
[CreateAssetMenu(menuName = "Item/ToolData")]
public class ToolData : GearData
{
    [SerializeField, Header("持っている間、自身のステータスへ加える補正")] private StatModifier[] _statModifiers;
    public IReadOnlyList<StatModifier> StatModifiers => _statModifiers;

    public override ItemModel CreateModel() { return new ToolModel(this); }
}
