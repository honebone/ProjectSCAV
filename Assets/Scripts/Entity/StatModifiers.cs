using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity自身・銃・投射物の3種のステータス補正をまとめたもの
/// ImplantData/ToolData/BuffDataなど、パッシブ効果を持つデータが1つのフィールドとして持つ
/// 適用/除去はExtensions.ApplyTo/RemoveFromで一括して行う
/// </summary>
[System.Serializable]
public struct StatModifiers
{
    [SerializeField, Header("自身のステータスへ加える補正")] private EntityStatModifier[] _entityStatModifiers;
    [SerializeField, Header("銃のステータスへ加える補正")] private GunStatModifier[] _gunStatModifiers;
    [SerializeField, Header("投射物のステータスへ加える補正")] private PjtlStatModifier[] _pjtlStatModifiers;

    public IReadOnlyList<EntityStatModifier> EntityStatModifiers => _entityStatModifiers;
    public IReadOnlyList<GunStatModifier> GunStatModifiers => _gunStatModifiers;
    public IReadOnlyList<PjtlStatModifier> PjtlStatModifiers => _pjtlStatModifiers;
}
