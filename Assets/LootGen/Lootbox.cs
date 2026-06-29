using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートボックスの実体
/// インベントリの保持と、開封時のUI表示を担う
/// 中身の生成は LootGeneratorModel が行い、Init() で受け取る
/// </summary>
public class Lootbox : MonoBehaviour
{
    [SerializeField] private float _costMultiplier = 1;
    [SerializeField] private int _inventorySize;
    /// <summary>
    /// この確率(%)で全てのタグのアイテムが排出される(アイテムごとに抽選)
    /// </summary>
    [SerializeField] private int _incluideAllChance;
    [SerializeField] private List<ItemTagCandidate> _tagCandidates;

    public float CostMultiplier => _costMultiplier;
    public int InventorySize => _inventorySize;
    public IReadOnlyList<ItemTagCandidate> TagCandidates => _tagCandidates;

    private InventoryModel _loot;
    public InventoryModel Loot => _loot;

    /// <summary>
    /// LootGeneratorModel が生成したインベントリを受け取って初期化する
    /// </summary>
    public void Init(InventoryModel loot)
    {
        _loot = loot;
    }
}

[System.Serializable]
public struct ItemTagCandidate
{
    [SerializeField] private ItemTag _itemTag;
    [SerializeField] private int _weight;

    public ItemTag ItemTag => _itemTag;
    public int Weight => _weight;
}
