using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア全体の初期化・生成フローを統括する MonoBehaviour
/// ILootboxSpawner を実装し、LootGeneratorModel からのSpawn要求を処理する
/// </summary>
public class AreaManager : MonoBehaviour, ILootboxSpawner
{
    [SerializeField] private MapSpawner _mapSpawner;
    [SerializeField] private NavGraphScanner _navGraphScanner;
    [SerializeField] private Transform _rootBoxRoot;

    private AreaManagerModel _model;

    public void Init(AreaData areaData)
    {
        // マップ生成
        _mapSpawner.SpawnMap(areaData.RoomDatabase);

        // NavGraph構築
        _navGraphScanner.BuildNavGraph();

        // Model初期化（自身をILootboxSpawnerとして注入）
        // ItemDatabaseは起動時にGameBootstrapperがロード済みのものを参照する
        _model = new AreaManagerModel(GameBootstrapper.ItemDatabase, this);

        // ルート生成
        List<ILootboxSpawnPoint> spawnPoints = CollectSpawnPoints();
        _model.LootGenerator.GenerateLoot(spawnPoints, areaData.TotalLootCost, areaData.BaseCostPerLootbox);
    }

    // -------------------------------------------------------
    // ILootboxSpawner
    // -------------------------------------------------------

    public Lootbox Spawn(GameObject prefab, Vector2 position)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, _rootBoxRoot);
        return obj.GetComponent<Lootbox>();
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

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

        DevLog.Log($"[AreaManager] {result.Count} つのマーカーを取得");
        return result;
    }
}