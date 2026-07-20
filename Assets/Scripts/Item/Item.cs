using System.Collections.Generic;
using UnityEngine;

public class ItemModel
{
    private ItemData _data;
    public ItemData Data => _data;
    public string Name => _data.ItemName;
    public Vector2Int Size => _data.Size;
    public float CostMultiplier => _data.CostMultiplier;
    public IReadOnlyList<ItemTag> ItemTags => _data.ItemTags;
    public int MaxStack => _data.MaxStack;

    /// <summary>1スタック分のコスト = マス数 × 基本コスト × 補正</summary>
    public float StackCost
    {
        get
        {
            int tileCount = Size.x * Size.y;
            float stackCorrection = 1f - (1f - Constants.Instance.MaxStack1CostMultiplier) / MaxStack;
            float sizeCorrection = 1f + (tileCount - 1) * Constants.Instance.SizeCostMultiplier;
            return tileCount * Constants.Instance.ItemCosts[(int)_data.Rarity] * stackCorrection * sizeCorrection * CostMultiplier;
        }
    }

    /// <summary>1個あたりのコスト</summary>
    public float UnitCost => StackCost / MaxStack;

    public ItemModel(ItemData data) { _data = data; }

    public bool IsSameItem(ItemModel other) => other != null && _data == other._data;
}