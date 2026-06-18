using UnityEngine;

/// <summary>
/// インプラントのScriptableObject
/// 装備時にEntityModelへパッシブ効果を付与する
/// </summary>
[CreateAssetMenu(menuName = "Item/ImplantData")]
public class ImplantData : EquippableItemData
{
    [SerializeField] private ImplantPart _implantPart;
    public ImplantPart ImplantPart => _implantPart;
    public override ItemModel CreateModel() { return new ImplantModel(this); }
}

/// <summary>
/// インプラントの装着部位
/// エンティティ全種共通
/// </summary>
public enum ImplantPart { Head, Body, Arm, Leg }