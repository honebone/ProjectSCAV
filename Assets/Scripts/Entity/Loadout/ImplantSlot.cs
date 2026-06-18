using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantSlot
{
    private readonly ImplantPart _part;
    private ImplantModel _equipped;

    public ImplantPart Part => _part;
    public ImplantModel Equipped => _equipped;
    public bool IsEmpty => _equipped == null;

    public ImplantSlot(ImplantPart part)
    {
        _part = part;
    }

    /// <summary>
    /// 装備する
    /// すでに装備中のインプラントがあれば取り外してから新しいインプラントを装備する
    /// </summary>
    public bool TryEquip(ImplantModel implant, EntityModel owner)
    {
        if (implant == null) return false;
        if (implant.ImplantPart != _part) return false;

        _equipped?.OnUnequip(owner);
        _equipped = implant;
        _equipped.OnEquip(owner);
        return true;
    }

    /// <summary>
    /// 装備を外す
    /// 外したインプラントを返す（インベントリへの戻し処理は呼び出し側が行う）
    /// </summary>
    public ImplantModel Unequip(EntityModel owner)
    {
        if (IsEmpty) return null;

        ImplantModel item = _equipped;
        _equipped.OnUnequip(owner);
        _equipped = null;
        return item;
    }
}
