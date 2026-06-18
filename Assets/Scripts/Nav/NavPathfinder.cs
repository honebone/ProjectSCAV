using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NavGraph上でA*を実行して経路を返す pure C# クラス
/// </summary>
public class NavPathfinder
{
    private readonly NavGraph _graph;
    private readonly float _tileSize;
    private readonly Func<Vector2Int, bool> _hasTile;

    /// <param name="graph">探索対象のNavGraph</param>
    /// <param name="tileSize">タイル1マスのワールド単位サイズ（座標変換に使用）</param>
    public NavPathfinder(NavGraph graph, float tileSize, Func<Vector2Int, bool> hasTile)
    {
        _graph = graph;
        _tileSize = tileSize;
        _hasTile = hasTile;
    }

    /// <summary>
    /// ワールド座標を受け取り、最近傍ノード間でA*を実行して足場間移動の経路を返す。
    /// 経路先頭・末尾の連続するWalkエッジは除去し、足場間移動に必要な部分のみ残す。
    /// 自身と目標が同一足場上にある場合は NavPath.OnSameGround を返す。
    /// 到達不能な場合は NavPath.Unreachable を返す。
    /// </summary>
    public NavPath FindPath(Vector2 from, Vector2 to, int jumpHeight, int jumpWidth)
    {
        Vector2Int fromCell = WorldToCell(from);
        Vector2Int toCell = ResolveGroundCell(WorldToCell(to));

        NavNode start = FindNearestNode(fromCell);
        NavNode goal = FindNearestNode(toCell);

        if (start == null || goal == null) return NavPath.Unreachable;

        // 同一ノード、または同一足場上（start-goal間が地続きWalk）なら OnSameGround を返す
        if (start == goal || IsOnSameGround(start.Cell, goal.Cell))
            return NavPath.OnSameGround;

        NavPath raw = AStar(start, goal, jumpHeight, jumpWidth);
        if (!raw.IsReachable) return NavPath.Unreachable;

        return TrimWalkEdges(raw);
    }

    public bool IsOnSameGround(Vector2 posA, Vector2 posB)
    {
        Vector2Int cellA = ResolveGroundCell(WorldToCell(posA));
        Vector2Int cellB = ResolveGroundCell(WorldToCell(posB));
        return cellA.y == cellB.y && IsGroundContinuous(cellA, cellB);
    }

    // -------------------------------------------------------
    // トリミング
    // -------------------------------------------------------

    /// <summary>
    /// A*が返した生の経路から、先頭・末尾の連続するWalkエッジを除去する。
    /// 経路はノードのリストであり、ノード[i]→ノード[i+1]のエッジ種別をもとに判定する。
    /// 除去後にノードが1つ以下になった（つまり全てWalkだった）場合は OnSameGround を返す。
    /// </summary>
    private NavPath TrimWalkEdges(NavPath raw)
    {
        List<NavNode> nodes = new List<NavNode>(raw.Nodes);

        // 先頭のWalkエッジを除去
        // ノード[0]→ノード[1]のエッジがWalkである限り、ノード[0]を除去する
        while (nodes.Count >= 2)
        {
            NavEdge firstEdge = nodes[0].GetEdge(nodes[1]);
            if (firstEdge == null || firstEdge.Type != EdgeType.Walk) break;
            nodes.RemoveAt(0);
        }

        // 末尾のWalkエッジを除去
        // ノード[n-2]→ノード[n-1]のエッジがWalkである限り、ノード[n-1]を除去する
        while (nodes.Count >= 2)
        {
            NavEdge lastEdge = nodes[nodes.Count - 2].GetEdge(nodes[nodes.Count - 1]);
            if (lastEdge == null || lastEdge.Type != EdgeType.Walk) break;
            nodes.RemoveAt(nodes.Count - 1);
        }

        // トリミング後にノードが1つ以下ならば、移動はすべて同一足場上で完結する
        //if (nodes.Count <= 1) return NavPath.OnSameGround;
        if (nodes.Count == 0) return NavPath.OnSameGround;

        return new NavPath(nodes);
    }


    // -------------------------------------------------------
    // A*
    // -------------------------------------------------------

    private NavPath AStar(NavNode start, NavNode goal, int jumpHeight, int jumpWidth)
    {
        // コスト管理
        Dictionary<NavNode, float> gCost = new Dictionary<NavNode, float>();
        Dictionary<NavNode, float> fCost = new Dictionary<NavNode, float>();
        Dictionary<NavNode, NavNode> cameFrom = new Dictionary<NavNode, NavNode>();

        // 未処理ノードのリスト（優先度付きキューの代わりにListで管理）
        List<NavNode> open = new List<NavNode>();
        HashSet<NavNode> closed = new HashSet<NavNode>();

        gCost[start] = 0f;
        fCost[start] = Heuristic(start, goal);
        open.Add(start);

        while (open.Count > 0)
        {
            NavNode current = GetLowestFCost(open, fCost);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            open.Remove(current);
            closed.Add(current);

            foreach (NavEdge edge in current.Edges)
            {
                NavNode neighbor = edge.To;

                if (closed.Contains(neighbor)) continue;

                // 敵のジャンプ能力でこのエッジを使えるか確認
                if (!CanUseEdge(edge, jumpHeight, jumpWidth)) continue;

                float tentativeG = gCost[current] + edge.Cost;

                if (!gCost.ContainsKey(neighbor) || tentativeG < gCost[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gCost[neighbor] = tentativeG;
                    fCost[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }

        return NavPath.Unreachable;
    }

    /// <summary>エッジが敵のジャンプ能力で使用可能か</summary>
    private bool CanUseEdge(NavEdge edge, int jumpHeight, int jumpWidth)
    {
        if (edge.Type != EdgeType.Jump) return true;
        return edge.RequiredJumpHeight <= jumpHeight &&
               edge.RequiredJumpWidth <= jumpWidth;
    }

    /// <summary>ヒューリスティック：ユークリッド距離</summary>
    private float Heuristic(NavNode a, NavNode b)
    {
        int dx = Mathf.Abs(a.Cell.x - b.Cell.x);
        int dy = Mathf.Abs(a.Cell.y - b.Cell.y);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>openリストの中でfCostが最も低いノードを返す</summary>
    private NavNode GetLowestFCost(List<NavNode> open, Dictionary<NavNode, float> fCost)
    {
        NavNode lowest = open[0];
        foreach (NavNode node in open)
        {
            if (fCost.TryGetValue(node, out float f) &&
                f < fCost.GetValueOrDefault(lowest, float.MaxValue))
                lowest = node;
        }
        return lowest;
    }

    /// <summary>経路を復元してNavPathとして返す</summary>
    private NavPath ReconstructPath(Dictionary<NavNode, NavNode> cameFrom, NavNode current)
    {
        List<NavNode> path = new List<NavNode>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(current); // startノードを追加
        path.Reverse();
        return new NavPath(path);
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    /// <summary>
    /// 全ノードから最も近いノードを線形探索で返す
    /// </summary>
    private NavNode FindNearestNode(Vector2Int cell)
    {
        // 足場上でなければ真下を走査して足場上のセルを起点とする
        Vector2Int origin = ResolveGroundCell(cell);

        NavNode nearest = null;
        float minDist = float.MaxValue;

        foreach (NavNode node in _graph.Nodes.Values)
        {
            // 同じY座標（同じ高さ）でなければスキップ
            if (node.Cell.y != origin.y) continue;

            // 起点からノードまで地続きでなければスキップ（壁越し除外）
            if (!IsGroundContinuous(origin, node.Cell)) continue;

            float dist = Mathf.Abs(node.Cell.x - origin.x);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }

        return nearest;
    }

    /// <summary>
    /// セル座標が空中（下にタイルがない）の場合、真下を走査して足場上のセルを返す
    /// maxScanはjumpHeightを流用
    /// </summary>
    private Vector2Int ResolveGroundCell(Vector2Int cell)
    {
        // すでに歩行可能（下にタイルがあり自分は空）ならそのまま返す
        if (IsWalkable(cell)) return cell;

        // 真下を走査して最初の歩行可能セルを返す
        for (int dy = 1; dy <= Constants.Instance.RoomHeight; dy++)
        {
            Vector2Int below = cell + new Vector2Int(0, -dy);

            // タイルに阻まれたら終了（壁の中に入った）
            if (_hasTile(below)) break;

            if (IsWalkable(below)) return below;
        }

        return cell;
    }

    private bool IsWalkable(Vector2Int cell)
        => _hasTile(cell + Vector2Int.down) && !_hasTile(cell);

    /// <summary>
    /// from と to の間（同じY行）の全マスが歩行可能かチェック
    /// </summary>
    private bool IsGroundContinuous(Vector2Int from, Vector2Int to)
    {
        if (from.y != to.y) return false;

        int xMin = Mathf.Min(from.x, to.x);
        int xMax = Mathf.Max(from.x, to.x);
        for (int x = xMin; x <= xMax; x++)
        {
            if (!IsWalkable(new Vector2Int(x, from.y))) return false;
        }
        return true;
    }

    /// <summary>ワールド座標をグローバルセル座標に変換する</summary>
    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / _tileSize),
            Mathf.FloorToInt(worldPos.y / _tileSize));
    }
}
