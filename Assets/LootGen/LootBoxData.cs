using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LootData")]
public class LootBoxData : ScriptableObject
{
    [SerializeField]private GameObject _prefab;
    [SerializeField] private float _costMultiplier = 1;
    [SerializeField] private int _inventorySize;
    /// <summary>
    /// この確率(%)で全てのタグのアイテムが排出される(アイテムごとに抽選)
    /// </summary>
    [SerializeField] private int _includeAllChance;
    [SerializeField] private List<ItemTagCandidate> _tagCandidates;

    public GameObject Prefab => _prefab;
    public float CostMultiplier => _costMultiplier;
    public int InventorySize => _inventorySize;
    public int IncludeAllChance => _includeAllChance;
    public IReadOnlyList<ItemTagCandidate> TagCandidates => _tagCandidates;
}
