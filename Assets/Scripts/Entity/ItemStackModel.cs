using UnityEngine;

public class ItemStackModel
{
    private readonly ItemModel _item;
    private int _amount;

    public ItemModel Item => _item;
    public int Amount => _amount;
    public int MaxStack=>_item.MaxStack;
    public bool IsFull => _amount >= _item.MaxStack;
    public Vector2Int Size => _item.Size;

    public ItemStackModel(ItemModel item, int amount)
    {
        _item = item;
        _amount = amount;
    }

    public void Add(int amount) { _amount += amount; }
    public void Remove(int amount) { _amount -= amount; }
}
