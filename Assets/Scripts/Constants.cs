using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ConstantsData", fileName = "ConstantsData")]
public class Constants : ScriptableObject
{
    [SerializeField] private float _tileSize;
    [SerializeField] private int _roomWidth = 44;
    [SerializeField] private int _roomheight = 24;
    public float TileSize => _tileSize;
    public int RoomWidth => _roomWidth;
    public int RoomHeight => _roomheight;
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
