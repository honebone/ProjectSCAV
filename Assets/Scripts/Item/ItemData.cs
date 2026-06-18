using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------
// 基底
// -------------------------------------------------------

/// <summary>
/// 全アイテムの基底ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Rarity _rarity;
    [SerializeField] private List<ItemTag> _itemTags;
    [SerializeField] private Vector2Int _size = Vector2Int.one;
    [SerializeField] private int _maxStack = 1;
    [SerializeField] private int _price;

    public string ItemName => _itemName;
    public Sprite Sprite => _sprite;
    public Rarity Rarity => _rarity;
    public IReadOnlyList<ItemTag> ItemTags => _itemTags;
    public Vector2Int Size => _size;
    public int MaxStack => _maxStack;
    public int Price => _price;

    public virtual ItemModel CreateModel() { return new ItemModel(this); }
}

public enum Rarity { common,uncommon,rare,epic,legendary}
public enum ItemTag { junk, valuable,military, gun,bullet, implant, installer,tool,medical,}
