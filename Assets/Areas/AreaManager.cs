using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア全体の初期化・生成フローを統括する MonoBehaviour
/// マップ生成・NavGraph構築・ルート生成の開始命令を担う
/// 各生成ロジックは MapSpawner・NavGraphScanner・LootGeneratorModel が持つ
/// </summary>
public class AreaManager : MonoBehaviour
{
    [SerializeField] private MapSpawner _mapSpawner;
    [SerializeField] private NavGraphScanner _navGraphScanner;

    private AreaManagerModel _model;

    //private void Start()
    //{
    //    Init();
    //}

    public void Init(AreaData areaData)
    {
        // マップ生成
        _mapSpawner.SpawnMap(areaData.RoomDatabase);

        // NavGraph構築
        _navGraphScanner.BuildNavGraph();


        _model = new AreaManagerModel(areaData);

        // ルート生成
        List<ILootboxSpawnPoint> spawnPoints = CollectSpawnPoints();
        _model.LootGenerator.GenerateLoot(spawnPoints, areaData.TotalLootCost, areaData.BaseCostPerLootbox);
    }

    /// <summary>
    /// 生成済みの全部屋から ILootboxSpawnPoint を収集して返す
    /// </summary>
    private List<ILootboxSpawnPoint> CollectSpawnPoints()
    {
        List<ILootboxSpawnPoint> result = new List<ILootboxSpawnPoint>();

        foreach (Room room in _mapSpawner.RoomsRoot.GetComponentsInChildren<Room>())
        {
            foreach (ILootboxSpawnPoint point in room.SpawnPoints)
            {
                result.Add(point);
            }
        }

        DevLog.Log($"{result.Count} つのマーカーを取得");
        return result;
    }
}