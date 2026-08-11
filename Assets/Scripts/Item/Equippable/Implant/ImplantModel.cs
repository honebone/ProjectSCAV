using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// インプラントのModel / IPassive
/// 装備している間、EntityModelへパッシブ効果を付与する
/// 既定ではDataのステータス補正を自動適用するのみ。それ以外の独自効果を持たせたい場合はこのクラスを継承しoverrideする
/// </summary>
public class ImplantModel : EquippableItemModel, IPassive
{
    private ImplantData _data;
    public ImplantPart ImplantPart => _data.ImplantPart;
    public ImplantModel(ImplantData data) : base(data)
    {
        _data = data;
    }

    /// <summary>インプラントスロットに装備したとき</summary>
    public virtual void OnApply(EntityModel owner)
    {
        SetEquipped(true);
        _data.StatModifiers.ApplyTo(owner);
    }

    /// <summary>インプラントスロットから外したとき</summary>
    public virtual void OnRemove(EntityModel owner)
    {
        SetEquipped(false);
        _data.StatModifiers.RemoveFrom(owner);
    }
}
