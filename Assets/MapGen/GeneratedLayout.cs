using UnityEngine;

/// <summary>
/// マップ生成結果を保持するデータクラス
/// このクラス自身はロジックを持たない
/// </summary>
public class GeneratedLayout
{
    private readonly RoomData[,] _grid;
    private readonly Vector2Int _startPos;
    private int _roomCount;

    public RoomData[,] Grid => _grid;
    public Vector2Int StartPos => _startPos;
    public int RoomCount => _roomCount;
    public int GridSize => _grid.GetLength(0);

    public GeneratedLayout(int gridSize)
    {
        _grid = new RoomData[gridSize, gridSize];
        _startPos = new Vector2Int(gridSize / 2, gridSize / 2);
    }

    /// <summary>グリッド座標が有効な範囲内かどうか</summary>
    public bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < GridSize &&
               pos.y >= 0 && pos.y < GridSize;
    }

    /// <summary>指定座標にRoomDataを配置し部屋数をインクリメントする</summary>
    public void Place(Vector2Int pos, RoomData room)
    {
        _grid[pos.x, pos.y] = room;
        _roomCount++;
    }

    /// <summary>指定座標のRoomDataを返す。範囲外またはnullの場合はnullを返す</summary>
    public RoomData Get(Vector2Int pos)
    {
        if (!InBounds(pos)) return null;
        return _grid[pos.x, pos.y];
    }
}