using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装備スロットに入れられるアイテムの基底Model
/// </summary>
public abstract class EquippableItemModel : ItemModel
{
    private bool _isEquipped;
    public bool IsEquipped => _isEquipped;

    /// <summary>装備時に呼ばれる</summary>
    public virtual void Subscribe(EntityModel owner) { }
    /// <summary>装備解除時に呼ばれる</summary>
    public virtual void Unsubscribe(EntityModel owner) { }

    protected EquippableItemModel(EquippableItemData data) : base(data) { }

    protected void SetEquipped(bool value) { _isEquipped = value; }
}