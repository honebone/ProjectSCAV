using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// インプラントのScriptableObject
/// 装備するとEntityModelへパッシブ効果を付与する
/// ステータス補正だけで表現できるインプラントは、Modifiersを設定するだけでよい（C#クラスを分ける必要はない）
/// </summary>
[CreateAssetMenu(menuName = "Item/ImplantData")]
public class ImplantData : EquippableItemData
{
    [SerializeField] private ImplantPart _implantPart;
    [SerializeField, Header("装備している間、自身/銃/投射物のステータスへ加える補正")] private StatModifiers _modifiers;

    public ImplantPart ImplantPart => _implantPart;
    public StatModifiers Modifiers => _modifiers;

    public override ItemModel CreateModel() { return new ImplantModel(this); }
}

/// <summary>
/// インプラントの装備部位
/// エンティティ全種共通
/// </summary>
public enum ImplantPart { Shield, Strage, Booster, Cooler }
