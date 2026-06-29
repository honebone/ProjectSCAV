using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部屋プレハブが所持するコンポーネント
/// 子オブジェクトから LootboxMarker を収集し、生成候補地点のリストを提供する
/// </summary>
public class Room : MonoBehaviour
{
    [SerializeField] private Transform _markersP;
    [SerializeField] private Transform _propsP;
    private List<ILootboxSpawnPoint> _spawnPoints;
    public IReadOnlyList<ILootboxSpawnPoint> SpawnPoints => _spawnPoints;

    private void Awake()
    {
        // 子オブジェクトの LootboxMarker をすべて収集する
        LootboxMarker[] markers = _markersP.GetComponentsInChildren<LootboxMarker>();
        _spawnPoints = new List<ILootboxSpawnPoint>(markers);
    }
}