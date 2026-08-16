/// <summary>
/// インプラント用装備スロット
/// コンストラクタで指定した部位(ImplantPart)に一致するインプラントのみ装備できる
/// </summary>
public class ImplantSlot
{
    private readonly ImplantPart _part;
    private ItemStackModel _equipped;

    public ImplantPart Part => _part;
    public ItemStackModel Equipped => _equipped;
    public bool IsEmpty => _equipped == null;

    public ImplantSlot(ImplantPart part)
    {
        _part = part;
    }

    /// <summary>このスロットに装備できるか判定（部位が一致するImplantModelのみ可）</summary>
    public bool CanEquip(ItemStackModel stack) => stack.Item is ImplantModel implant && implant.ImplantPart == _part;

    /// <summary>装備中のインプラントのパッシブ効果を毎フレーム更新する</summary>
    public void Tick(float deltaTime, EntityModel user)
    {
        if (!IsEmpty && _equipped.Item is ImplantModel implant)
        {
            implant.OnTick(deltaTime, user);
        }
    }

    /// <summary>
    /// 装備して以前装備していたインプラントを返す
    /// </summary>
    public ItemStackModel TryEquip(ItemStackModel stack, EntityModel owner)
    {
        if (stack == null) return null;
        if (!CanEquip(stack)) return null;

        ItemStackModel prev = _equipped;

        GetImplant()?.OnRemove(owner);
        _equipped = stack;
        GetImplant()?.OnApply(owner);

        return prev;
    }

    /// <summary>
    /// 装備を外す
    /// 外したインプラントを返す（インベントリへの戻し処理は呼び出し側が行う）
    /// </summary>
    public ItemStackModel Unequip(EntityModel owner)
    {
        if (IsEmpty) return null;

        ItemStackModel stack = _equipped;
        GetImplant()?.OnRemove(owner);
        _equipped = null;
        return stack;
    }

    private ImplantModel GetImplant()
    {
        if (_equipped == null || _equipped.Item is not ImplantModel) return null;
        return _equipped.Item as ImplantModel;
    }
}
