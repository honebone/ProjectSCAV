using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// ナビゲーショングラフの頂点
/// グローバルセル座標とそこから伸びる接続リストを保持する
/// </summary>
public class NavNode
{
    private readonly Vector2Int _cell;
    private readonly List<NavEdge> _edges;

    public Vector2Int Cell => _cell;
    public IReadOnlyList<NavEdge> Edges => _edges;
    public Vector2 WorldPos => GetWorldPos();

    public NavNode(Vector2Int cell)
    {
        _cell = cell;
        _edges = new List<NavEdge>();
    }

    public void AddEdge(NavEdge edge) { _edges.Add(edge); }
    public NavEdge GetEdge(NavNode to)
    {
        List<NavEdge> found = _edges.Where(e => e.To == to).ToList();
        if (found.Count == 0)
        {
            DevLog.Error("[NavNode] 次のノードに繋がるNavEdgeが見つかりませんでした");
            return null;
        }
        if (found.Count > 1)
        {
            DevLog.Warning($"[NavNode] 次のノードに繋がるNavEdgeが{found.Count}つ見つかりました");
            string s = $"from({_cell})\n";
            _edges.ForEach(e => s += $"{e.GetInfo()}\n");
            DevLog.Warning(s);
        }
        return found[0];
    }

    private Vector2 GetWorldPos()
    {
        float tileSize = Constants.Instance.TileSize;

        return new Vector2(_cell.x + 0.5f, _cell.y + 0.5f) * tileSize;
    }
}