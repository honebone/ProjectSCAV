using UnityEngine;

/// <summary>
/// GeneratedLayout をもとに実際に部屋プレハブを Instantiate するクラス
/// 生成ロジック(MapGenerator)と配置処理を分離する
/// </summary>
public class MapSpawner : MonoBehaviour
{
    // 部屋サイズ(タイル数)。44x24タイル固定
    // NavGraphScanner からも参照するため public const に変更
    public const int RoomTileWidth = 44;
    public const int RoomTileHeight = 24;

    [SerializeField] private MapGenerator _generator;

    /// <summary>タイル1マスのワールド単位サイズ（UnityのGrid設定に合わせる）</summary>
    [SerializeField] private float _tileSize = 1f;

    /// <summary>生成した部屋オブジェクトの親Transform</summary>
    [SerializeField] private Transform _roomsRoot;

    private GeneratedLayout _currentLayout;

    public GeneratedLayout CurrentLayout => _currentLayout;

    /// <summary>NavGraphScanner がTilemapを収集するために参照する</summary>
    public Transform RoomsRoot => _roomsRoot;

    public float TileSize => _tileSize;

    // -------------------------------------------------------
    // 外部からの生成エントリーポイント
    // -------------------------------------------------------

    public void SpawnMap()
    {
        ClearRooms();

        _currentLayout = _generator.Generate();
        if (_currentLayout == null)
        {
            DevLog.Error("[MapSpawner] GeneratedLayout の生成に失敗しました。");
            return;
        }

        int gridSize = _currentLayout.GridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                RoomData data = _currentLayout.Grid[x, y];
                if (data == null) continue;
                if (data.Prefab == null)
                {
                    DevLog.Warning($"[MapSpawner] ({x},{y}) の RoomData にPrefabが設定されていません。スキップします。");
                    continue;
                }

                Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                GameObject room = Instantiate(data.Prefab, worldPos, Quaternion.identity, _roomsRoot);
                room.name = $"Room_{x}_{y}_{data.PassagePattern}";
            }
        }

        DevLog.Log($"[MapSpawner] 配置完了: {_currentLayout.RoomCount}部屋");
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    /// <summary>グリッド座標をワールド座標に変換する</summary>
    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * RoomTileWidth * _tileSize;
        float y = gridPos.y * RoomTileHeight * _tileSize;
        return new Vector3(x, y, 0f);
    }

    private void ClearRooms()
    {
        if (_roomsRoot == null) return;
        for (int i = _roomsRoot.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_roomsRoot.GetChild(i).gameObject);
        }
    }
}