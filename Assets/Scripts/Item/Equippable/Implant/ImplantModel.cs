using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantModel : EquippableItemModel
{
    private ImplantData _data;
    public ImplantPart ImplantPart => _data.ImplantPart;
    public ImplantModel(ImplantData data) : base(data)
    {
        _data = data;
    }

    /// <summary>インプラントスロットに装備したとき</summary>
    public virtual void OnEquip(EntityModel owner)
    {
        SetEquipped(true);
        // サブクラスでステータス補正などを実装する
    }

    /// <summary>インプラントスロットから取り外したとき</summary>
    public virtual void OnUnequip(EntityModel owner)
    {
        SetEquipped(false);
        // サブクラスでステータス補正の除去を実装する
    }
}
