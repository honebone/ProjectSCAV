using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ConstantsData", fileName = "ConstantsData")]
public class Constants : ScriptableObject
{
    [Header("Colors")]
    public Color Color_DMG;
    public Color Color_shieldDMG;

    [Header("\n\n\nRooms")]
    public float TileSize;
    public int RoomWidth;
    public int RoomHeight;

    [Header("\n\n\nEntities")]
    public float JumpInterval = 0.1f;
    public Vector2Int ParticlesOnDMG;
    public EntityWorldText WorldTextPrefab;
    public GameObject VE_Die;
    public GameObject VE_ShieldBreak;

    [Header("\n\n\nInventory")]
    public int InventorySlotsPerRow;
    public float MaxStack1CostMultiplier;
    public float SizeCostMultiplier;
    public float LootCostStdDevRatio;
    public float LootItemCountMeanRatio;
    public float LootItemCountStdDevRatio;
    public List<int> ItemCosts;
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
