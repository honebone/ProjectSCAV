using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LoadoutModel
{
    //固定アイテムスロットの取得添え字

    private int _holdIndex;
    private readonly EntityModel _owner;

    private readonly List<HoldableSlot> _holdableSlots;
    private readonly List<ImplantSlot> _implantSlots;
    private readonly PassiveModifierSet _passiveModifiers = new PassiveModifierSet();

    public HoldableItemModel HoldingItem => _holdableSlots[_holdIndex].Equipped?.Item as HoldableItemModel;
    public int HoldIndex => _holdIndex;
    public IReadOnlyList<HoldableSlot> HoldableSlots => _holdableSlots;
    public IReadOnlyList<ImplantSlot> ImplantSlots => _implantSlots;

    /// <summary>装備中パッシブ効果による銃・投射物ステータスへの補正の集計（Pull型）</summary>
    public PassiveModifierSet PassiveModifiers => _passiveModifiers;

    public event Action<HoldableItemModel> OnActiveItemChanged;

    /// <summary>ロードアウト内のいずれかの銃が発射されたとき（武器の持ち替えに関わらず購読できる安定したハブ）</summary>
    public event Action<FireParams> OnAnyGunFired;
    /// <summary>ロードアウト内のいずれかの銃のリロードが完了したとき（武器の持ち替えに関わらず購読できる安定したハブ）</summary>
    public event Action<float> OnAnyGunReloadCompleted;

    public void Tick(float deltaTime, EntityModel user)
    {
        _holdableSlots.ForEach(h =>
        {
            if (!h.IsEmpty) h.Tick(deltaTime, user);
        });

        //インプラントは装備/解除時にステータス補正を適用するのみで、現状Tickは不要
    }

    public LoadoutModel(int guns,int gears,IReadOnlyList<ImplantPart> implants, EntityModel owner)
    {
        _owner = owner;

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
            DevLog.Error($"[LoadoutModel] indexは範囲外です:{index}");
            return;
        }

        HoldableSlot slot = _holdableSlots[index];
        GunModel prevGun = slot.Equipped?.Item as GunModel;

        slot.TryEquip(itemStack, owner);

        // 実際にスロットの中身が変わった場合のみ、銃イベントハブの購読を張り替える
        // （itemStackがnullだったりCanEquipに失敗した場合は中身が変わらないため何もしない）
        GunModel newGun = slot.Equipped?.Item as GunModel;
        if (!ReferenceEquals(prevGun, newGun))
        {
            if (prevGun != null) UnsubscribeGun(prevGun);
            if (newGun != null) SubscribeGun(newGun);
        }

        if (index == _holdIndex) SwitchSlot(index);
    }

    /// <summary>指定した部位のインプラントスロットへ装備する</summary>
    public bool TryEquipImplant(int index, ImplantModel implant, EntityModel owner)
    {
        if (index < 0 || index >= _implantSlots.Count)
        {
            DevLog.Error($"[LoadoutModel] implant indexは範囲外です:{index}");
            return false;
        }

        return _implantSlots[index].TryEquip(implant, owner);
    }

    /// <summary>指定した部位のインプラントスロットから外す</summary>
    public ImplantModel UnequipImplant(int index, EntityModel owner)
    {
        if (index < 0 || index >= _implantSlots.Count)
        {
            DevLog.Error($"[LoadoutModel] implant indexは範囲外です:{index}");
            return null;
        }

        return _implantSlots[index].Unequip(owner);
    }

    public void SwitchSlot(int index)
    {
        HoldingItem?.OnUnhold(_owner);
        _holdIndex = index;
        OnActiveItemChanged?.Invoke(HoldingItem);
        HoldingItem?.OnHold(_owner);
    }
    public void SwitchNext()
    {
        if (_holdIndex == _holdableSlots.Count - 1) { SwitchSlot(0); }
        else { SwitchSlot(_holdIndex + 1); }
    }
    public void SwitchBack()
    {
        if (_holdIndex == 0) { SwitchSlot(_holdableSlots.Count-1); }
        else { SwitchSlot(_holdIndex - 1); }
    }

    // -------------------------------------------------------
    // 銃イベントの集約ハブ
    // ロードアウトに銃が入っている間は常に購読しておき、武器の持ち替え・新規入手の影響を受けない
    // パッシブ効果はGunModel個別ではなく、この集約イベントを購読する
    // -------------------------------------------------------

    private void SubscribeGun(GunModel gun)
    {
        gun.OnFired += HandleAnyGunFired;
        gun.OnReloadCompleted += HandleAnyGunReloadCompleted;
    }

    private void UnsubscribeGun(GunModel gun)
    {
        gun.OnFired -= HandleAnyGunFired;
        gun.OnReloadCompleted -= HandleAnyGunReloadCompleted;
    }

    private void HandleAnyGunFired(FireParams fireParams) => OnAnyGunFired?.Invoke(fireParams);
    private void HandleAnyGunReloadCompleted(float reloadTime) => OnAnyGunReloadCompleted?.Invoke(reloadTime);
}
