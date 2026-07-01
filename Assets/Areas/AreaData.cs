using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AreaData")]
public class AreaData : ScriptableObject
{
    [SerializeField] private RoomDatabase _roomDatabase;
    [SerializeField] private int _totalLootCost;
    [SerializeField] private int _baseCostPerLootbox;

    public RoomDatabase RoomDatabase => _roomDatabase;
    public int TotalLootCost => _totalLootCost;
    public int BaseCostPerLootbox => _baseCostPerLootbox;
}
