using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ConstantsData", fileName = "ConstantsData")]
public class Constants : ScriptableObject
{
    [Header("Rooms")]
    [SerializeField] private float _tileSize;
    [SerializeField] private int _roomWidth = 44;
    [SerializeField] private int _roomheight = 24;

    [Header("\n\n\nEntities")]
    [SerializeField] private Vector2Int _particlesOnDMG;

    [Header("\n\n\nInventory")]
    [SerializeField] private List<int> _itemCosts;

    [SerializeField] private int _inventorySlotsPerRow = 10;
    // 既存の _inventorySlotsPerRow に加えて
    [SerializeField] private float _maxStack1CostMultiplier = 0.75f; // maxStackが1のときの倍率補正
    [SerializeField] private float _lootCostStdDevRatio;    // ロトボックス目標コストに対する標準偏差の割合
    [SerializeField] private float _lootItemCountMeanRatio; // maxStackに対する排出個数平均の割合
    [SerializeField] private float _lootItemCountStdDevRatio; // 個数平均に対する標準偏差の割合

    public float TileSize => _tileSize;
    public int RoomWidth => _roomWidth;
    public int RoomHeight => _roomheight;
    public Vector2Int ParticlesOnDMG => _particlesOnDMG;
    public int InventorySlotsPerRow => _inventorySlotsPerRow;
    public float MaxStack1CostMultiplier => _maxStack1CostMultiplier;
    public float LootCostStdDevRatio => _lootCostStdDevRatio;
    public float LootItemCountMeanRatio => _lootItemCountMeanRatio;
    public float LootItemCountStdDevRatio => _lootItemCountStdDevRatio;
    public List<int> ItemCosts => _itemCosts;
    //[Header("\n\n\nLootbox")]
    //public List<ItemData> _Item
    // -------------------------------------------------------
    // シングルトンAPI
    // -------------------------------------------------------

    private const string ResourcePath = "ConstantsData";
    private static Constants _instance;

    public static Constants Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = Resources.Load<Constants>(ResourcePath);

            if (_instance == null)
            {
                DevLog.Error($"[Costants] Resources/{ResourcePath}.asset が見つかりません。");
            }

            return _instance;
        }
    }
}
