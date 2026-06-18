using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// MapSpawner完了後に NavGraphBuilder を呼び出し、StageContext を構築する
/// </summary>
public class NavGraphScanner : MonoBehaviour
{
    [SerializeField] private MapSpawner _mapSpawner;

    [Header("ジャンプ設定")]
    [SerializeField] private int _maxJumpHeight = 4;
    [SerializeField] private int _maxJumpWidth = 6;
    [SerializeField] private float _jumpCostMultiplier = 1.5f;

    private StageContext _stageContext;
    public StageContext StageContext => _stageContext;
    private NavPathfinder _pathfinder;
    public NavPathfinder Pathfinder => _pathfinder;

    // -------------------------------------------------------
    // 外部からの構築エントリーポイント
    // -------------------------------------------------------

    /// <summary>
    /// MapSpawner.SpawnMap() 完了後に呼ぶ
    /// </summary>
    public void BuildNavGraph()
    {
        GeneratedLayout layout = _mapSpawner.CurrentLayout;
        if (layout == null)
        {
            DevLog.Error("[NavGraphScanner] CurrentLayout が null です。SpawnMap() を先に呼んでください。");
            return;
        }

        Func<Vector2Int, bool> hasTile = BuildHasTileFunc();
        RectInt bounds = CalcGlobalBounds(layout);

        NavGraphBuilder builder = new NavGraphBuilder(_jumpCostMultiplier);
        NavGraph graph = builder.Build(
            hasTile,
            bounds,
            _maxJumpHeight,
            _maxJumpWidth);

        _stageContext = new StageContext(layout, graph);
        DevLog.Log($"[NavGraphScanner] 構築完了: {graph.Nodes.Count} ノード");

        _pathfinder = new NavPathfinder(graph, Constants.Instance.TileSize, hasTile);
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    /// <summary>
    /// 全部屋の Tilemap（Ground レイヤーのみ）を収集し、
    /// グローバルセル座標 → タイル有無 を返す関数を生成する
    /// </summary>
    private Func<Vector2Int, bool> BuildHasTileFunc()
    {
        // Ground レイヤーのみを対象とする（Decoration は衝突判定なし）
        List<Tilemap> tilemaps = new List<Tilemap>();
        foreach (Tilemap tm in _mapSpawner.RoomsRoot.GetComponentsInChildren<Tilemap>())
        {
            // "Ground" という名前のTilemapのみ収集
            if (tm.gameObject.name == "Ground")
                tilemaps.Add(tm);
        }

        return (Vector2Int globalCell) =>
        {
            Vector3 worldPos = new Vector3(
                globalCell.x * _mapSpawner.TileSize,
                globalCell.y * _mapSpawner.TileSize,
                0f);

            foreach (Tilemap tm in tilemaps)
            {
                Vector3Int local = tm.WorldToCell(worldPos);
                if (tm.HasTile(local)) return true;
            }
            return false;
        };
    }

    /// <summary>
    /// マップ全体のスキャン範囲をグローバルセル座標で返す
    /// </summary>
    private RectInt CalcGlobalBounds(GeneratedLayout layout)
    {
        int xMin = -MapSpawner.RoomTileWidth / 2;
        int yMin = -MapSpawner.RoomTileHeight / 2;
        int w = layout.GridSize * MapSpawner.RoomTileWidth;
        int h = layout.GridSize * MapSpawner.RoomTileHeight;
        return new RectInt(xMin, yMin, w, h);
    }

}