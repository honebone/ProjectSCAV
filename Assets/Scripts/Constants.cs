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
    [SerializeField] private int _inventorySlotsPerRow = 10;
    public float TileSize => _tileSize;
    public int RoomWidth => _roomWidth;
    public int RoomHeight => _roomheight;
    public Vector2Int ParticlesOnDMG => _particlesOnDMG;

    public int InventorySlotsPerRow => _inventorySlotsPerRow;    
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
