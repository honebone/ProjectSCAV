using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadoutModel
{
    //固定アイテムスロットの実装忘れるな

    private int _holdIndex;

    private readonly List<HoldableSlot> _holdableSlots;
    private readonly List<ImplantSlot> _implantSlots;

    public HoldableItemModel HoldingItem => _holdableSlots[_holdIndex].Equipped?.Item as HoldableItemModel;
    public int HoldIndex => _holdIndex;
    public IReadOnlyList<HoldableSlot> HoldableSlots => _holdableSlots;
    public IReadOnlyList<ImplantSlot> ImplantSlots => _implantSlots;

    public event Action<HoldableItemModel> OnActiveItemChanged;

    public LoadoutModel(int guns,int gears,IReadOnlyList<ImplantPart> implants)
    {
        _holdableSlots = new List<HoldableSlot>();
        for (int i = 0; i < guns; i++)
            _holdableSlots.Add(new GunSlot());

        for (int i = 0; i < gears; i++)
            _holdableSlots.Add(new GearSlot());

        _implantSlots = new List<ImplantSlot>();
        foreach (ImplantPart part in implants)
            _implantSlots.Add(new ImplantSlot(part));

        SwitchSlot(0);
    }

    public void TryEquip(int index,ItemStackModel itemStack,EntityModel owner)
    {
        if (index < 0 || index >= _holdableSlots.Count)
        {
            DevLog.Error($"[LoadoutModel] indexが範囲外です:{index}");
            return;
        }

        _holdableSlots[index].TryEquip(itemStack, owner);
        if (index == _holdIndex) SwitchSlot(index);
    }

    public void SwitchSlot(int index)
    {
        _holdIndex = index;
        OnActiveItemChanged?.Invoke(HoldingItem);
    }
    public void SwitchNext()
    {
        DevLog.Log("SwitchNext");
        if (_holdIndex == _holdableSlots.Count - 1) { SwitchSlot(0); }
        else { SwitchSlot(_holdIndex + 1); }
    }
    public void SwitchBack()
    {
        DevLog.Log("SwitchBack");
        if (_holdIndex == 0) { SwitchSlot(_holdableSlots.Count-1); }
        else { SwitchSlot(_holdIndex - 1); }
    }
}