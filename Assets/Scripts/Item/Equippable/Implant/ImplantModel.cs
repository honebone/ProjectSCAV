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
        _data.Modifiers.ApplyTo(owner);
    }

    /// <summary>インプラントスロットから外したとき</summary>
    public virtual void OnRemove(EntityModel owner)
    {
        SetEquipped(false);
        _data.Modifiers.RemoveFrom(owner);
    }

    /// <summary>
    /// 装備している間、毎フレーム呼ばれる（既定では何もしない）
    /// 「立ち止まっている間だけ拡散ペナルティが低下する」のような、常時条件判定が必要なパッシブはこれをoverrideする
    /// </summary>
    public virtual void OnTick(float deltaTime, EntityModel owner) { }
}
