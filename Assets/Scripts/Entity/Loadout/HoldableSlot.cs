
public abstract class HoldableSlot
{
    private ItemStackModel _equipped;

    public ItemStackModel Equipped => _equipped;
    public bool IsEmpty => _equipped == null;

    /// <summary>このスロットに装備できるか判定（サブクラスで型チェック）</summary>
    public abstract bool CanEquip(ItemStackModel stack);

    /// <summary>装備して以前持っていたアイテムを返す</summary>
    public ItemStackModel TryEquip(ItemStackModel item, EntityModel owner)
    {
        if (item == null) return null;
        if (!CanEquip(item)) return null;

        ItemStackModel prev = _equipped;

        GetItem()?.Unsubscribe(owner);
        _equipped = item;
        GetItem()?.Subscribe(owner);

        return prev;
    }

    /// <summary>
    /// 装備を外す
    /// 外したアイテムを返す（インベントリへの戻し処理は呼び出し側が行う）
    /// </summary>
    public ItemStackModel Unequip(EntityModel owner)
    {
        if (IsEmpty) return null;

        ItemStackModel item = _equipped;
        GetItem()?.Unsubscribe(owner);
        _equipped = null;
        return item;
    }

    private HoldableItemModel GetItem()
    {
        if (_equipped == null || _equipped.Item is not HoldableItemModel) return null;
        return _equipped.Item as HoldableItemModel;
    }
}

/// <summary>
/// 消耗品・道具用スロット
/// </summary>
public class GearSlot : HoldableSlot
{
    public override bool CanEquip(ItemStackModel stack)
        => stack.Item is GearModel;
}

/// <summary>
/// 銃用スロット
/// </summary>
public class GunSlot : HoldableSlot
{
    public override bool CanEquip(ItemStackModel stack)
        => stack.Item is GunModel;
}
