using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel
{
    private readonly ItemData _data;

    public ItemData Data => _data;
    public int MaxStack => _data.MaxStack;
    public Vector2Int Size => _data.Size;

    public ItemModel(ItemData data)
    {
        _data = data;
    }

    /// <summary>同じItemDataを参照しているか（同一アイテム種別の判定）</summary>
    public bool IsSameItem(ItemModel other)
    {
        if (other == null) return false;
        return _data == other._data;
    }
}
