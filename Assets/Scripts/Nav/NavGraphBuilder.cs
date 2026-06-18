using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイルマップ情報から NavGraph を構築する pure C# クラス
/// Unity依存を持たず、hasTile デリゲートで外部からタイル判定を注入する
/// </summary>
public class NavGraphBuilder
{
    private readonly float _jumpCostMultiplier;

    public NavGraphBuilder(float jumpCostMultiplier = 1.5f)
    {
        _jumpCostMultiplier = jumpCostMultiplier;
    }

    /// <summary>
    /// グラフを構築して返す
    /// </summary>
    /// <param name="hasTile">グローバルセル座標にタイルがあるか判定する関数</param>
    /// <param name="scanBounds">スキャン対象範囲（グローバルセル座標）</param>
    /// <param name="maxJumpHeight">敵が飛べる最大ジャンプ高度（タイル数）</param>
    /// <param name="maxJumpWidth">敵が飛べる最大ジャンプ幅（タイル数）</param>
    public NavGraph Build(
        Func<Vector2Int, bool> hasTile,
        RectInt scanBounds,
        int maxJumpHeight,
        int maxJumpWidth)
    {
        NavGraph graph = new NavGraph();

        PlaceNodes(graph, hasTile, scanBounds, maxJumpHeight);
        BuildEdges(graph, hasTile, maxJumpHeight, maxJumpWidth);

        return graph;
    }

    // -------------------------------------------------------
    // ノード生成
    // -------------------------------------------------------

    private void PlaceNodes(
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        RectInt bounds,
        int maxJumpHeight)
    {
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!IsWalkable(cell, hasTile)) continue;
                if (ShouldPlaceNode(cell, hasTile))
                    graph.GetOrCreate(cell);

            }
        }

        // 崖端の外側から落下できる着地点にノードを追加
        PlaceLandingNodes(graph, hasTile, bounds, maxJumpHeight);
    }

    private bool ShouldPlaceNode(Vector2Int cell, Func<Vector2Int, bool> hasTile)
    {
        if (IsLeftEdge(cell, hasTile)) return true;
        if (IsRightEdge(cell, hasTile)) return true;
        if (IsNextToWall(cell, hasTile)) return true;
        return false;
    }

    /// <summary>
    /// 崖端の1マス外側から真下を maxJumpHeight マス走査し、
    /// 着地できる足場があればそのマス上にノードを追加する
    /// </summary>
    private void PlaceLandingNodes(
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        RectInt bounds,
        int maxJumpHeight)
    {
        // PlaceNodes で生成済みのノードをスナップショット化
        // （このメソッド内で追加したノードは走査対象に含めない）
        List<NavNode> edgeNodes = new List<NavNode>(graph.Nodes.Values);

        foreach (NavNode node in edgeNodes)
        {
            // 左端エッジ → 左に1マス外側から下を見る
            if (IsLeftEdge(node.Cell, hasTile))
                TryPlaceLanding(graph, hasTile, node.Cell + Vector2Int.left, maxJumpHeight);

            // 右端エッジ → 右に1マス外側から下を見る
            if (IsRightEdge(node.Cell, hasTile))
                TryPlaceLanding(graph, hasTile, node.Cell + Vector2Int.right, maxJumpHeight);
        }
    }

    /// <summary>
    /// origin の真下を走査し、最初に見つかった歩行可能マスにノードを置く
    /// タイルに阻まれるか maxJumpHeight を超えたら置かない
    /// </summary>
    private void TryPlaceLanding(
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        Vector2Int origin,
        int maxJumpHeight)
    {
        for (int dy = 1; dy <= maxJumpHeight; dy++)
        {
            Vector2Int below = origin + new Vector2Int(0, -dy);

            // タイルに阻まれたら終了
            if (hasTile(below)) break;

            // 歩行可能マス（下にタイルがあり自分は空）であればノードを追加
            if (IsWalkable(below, hasTile))
            {
                graph.GetOrCreate(below);
                break;
            }
        }
    }

    /// <summary>歩行可能セル：下にタイルがあり自分は空</summary>
    private bool IsWalkable(Vector2Int cell, Func<Vector2Int, bool> hasTile)
        => hasTile(cell + Vector2Int.down) && !hasTile(cell);

    /// <summary>足場左端：左と左下が空</summary>
    private bool IsLeftEdge(Vector2Int cell, Func<Vector2Int, bool> hasTile)
    {
        Vector2Int left = cell + Vector2Int.left;
        return !hasTile(left) && !hasTile(left + Vector2Int.down);
    }

    /// <summary>足場右端：右と右下が空</summary>
    private bool IsRightEdge(Vector2Int cell, Func<Vector2Int, bool> hasTile)
    {
        Vector2Int right = cell + Vector2Int.right;
        return !hasTile(right) && !hasTile(right + Vector2Int.down);
    }

    /// <summary>壁際：左右どちらかに壁タイルが隣接</summary>
    private bool IsNextToWall(Vector2Int cell, Func<Vector2Int, bool> hasTile)
        => hasTile(cell + Vector2Int.left) || hasTile(cell + Vector2Int.right);

    // -------------------------------------------------------
    // エッジ生成
    // -------------------------------------------------------

    private void BuildEdges(
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        int maxJumpHeight,
        int maxJumpWidth)
    {
        // ノードリストをスナップショット化（イテレーション中の変更を防ぐ）
        List<NavNode> nodes = new List<NavNode>(graph.Nodes.Values);

        foreach (NavNode node in nodes)
        {
            BuildWalkEdges(node, graph, hasTile);
            BuildFallEdges(node, graph, hasTile, maxJumpHeight);
            BuildJumpEdges(node, graph, nodes, hasTile, maxJumpHeight, maxJumpWidth);
        }
    }

    /// <summary>
    /// Walkエッジ：左右方向に最も近いノードと地続きであれば接続
    /// 地続き判定：自ノードと対象ノードの間の全マスが歩行可能
    /// </summary>
    private void BuildWalkEdges(NavNode node, NavGraph graph, Func<Vector2Int, bool> hasTile)
    {
        foreach (Vector2Int dir in new[] { Vector2Int.left, Vector2Int.right })
        {
            NavNode nearest = FindNearestNodeInDirection(node, dir, graph);
            if (nearest == null) continue;
            if (!IsGroundContinuous(node.Cell, nearest.Cell, hasTile)) continue;

            float cost = Mathf.Abs(nearest.Cell.x - node.Cell.x);
            node.AddEdge(new NavEdge(nearest, EdgeType.Walk, cost));
        }
    }

    /// <summary>
    /// 指定方向に1マスずつ進み、同じY行で最初に見つかったノードを返す
    /// マップ幅上限（部屋サイズ * グリッドサイズ）を超えたら null を返す
    /// </summary>
    private NavNode FindNearestNodeInDirection(NavNode node, Vector2Int dir, NavGraph graph)
    {
        const int SearchLimit = 400; // 44 * 9 = 396 を上回る値
        Vector2Int current = node.Cell + dir;

        for (int i = 0; i < SearchLimit; i++)
        {
            if (graph.TryGetNode(current, out NavNode found)) return found;
            current += dir;
        }

        return null;
    }

    /// <summary>
    /// from と to の間（同じY行）の全マスが歩行可能かチェック
    /// </summary>
    private bool IsGroundContinuous(Vector2Int from, Vector2Int to, Func<Vector2Int, bool> hasTile)
    {
        int xMin = Mathf.Min(from.x, to.x);
        int xMax = Mathf.Max(from.x, to.x);
        for (int x = xMin; x <= xMax; x++)
        {
            Vector2Int cell = new Vector2Int(x, from.y);
            if (!IsWalkable(cell, hasTile)) return false;
        }
        return true;
    }

    /// <summary>
    /// Fallエッジ：左端なら左列、右端なら右列を走査して着地ノードへ一方通行接続
    /// 両端（1マス幅の突起など）の場合は両方処理する
    /// </summary>
    private void BuildFallEdges(
        NavNode node,
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        int maxFallHeight)
    {
        bool left = IsLeftEdge(node.Cell, hasTile);
        bool right = IsRightEdge(node.Cell, hasTile);

        if (left) TryBuildFallEdge(node, graph, hasTile, maxFallHeight, Vector2Int.left);
        if (right) TryBuildFallEdge(node, graph, hasTile, maxFallHeight, Vector2Int.right);
    }

    /// <summary>
    /// 崖端の1マス外側の列を真下に走査し、最初の着地ノードへFallエッジを張る
    /// </summary>
    private void TryBuildFallEdge(
        NavNode node,
        NavGraph graph,
        Func<Vector2Int, bool> hasTile,
        int maxFallHeight,
        Vector2Int sideDir)
    {
        int scanX = node.Cell.x + sideDir.x;

        for (int dy = 1; dy <= maxFallHeight; dy++)
        {
            Vector2Int below = new Vector2Int(scanX, node.Cell.y - dy);

            if (hasTile(below)) break;

            if (!graph.TryGetNode(below, out NavNode target)) continue;

            float cost = dy;
            node.AddEdge(new NavEdge(target, EdgeType.Fall, cost));
            break;
        }
    }

    /// <summary>
    /// Jumpエッジ：ジャンプ範囲内の全ノードを総当たりで検査して接続
    /// dx==0、地続きノード、ジャンプ範囲外は除外
    /// </summary>
    private void BuildJumpEdges(
        NavNode node,
        NavGraph graph,
        List<NavNode> allNodes,
        Func<Vector2Int, bool> hasTile,
        int maxJumpHeight,
        int maxJumpWidth)
    {
        foreach (NavNode target in allNodes)
        {
            if (target == node) continue;

            int dx = Mathf.Abs(target.Cell.x - node.Cell.x);
            int dy = Mathf.Abs(target.Cell.y - node.Cell.y);
            int dyr = target.Cell.y - node.Cell.y; // 符号付き（正=上、負=下）

            // 真上・真下は除外
            if (dx == 0) continue;
            // ジャンプ範囲外
            if (dx > maxJumpWidth) continue;
            if (dy > maxJumpHeight) continue;
            // 同じ高さで地続きのノードはWalkで処理済みなので除外
            if (dyr == 0 && IsGroundContinuous(node.Cell, target.Cell, hasTile)) continue;
            //下方向で1マス横のノードはFallで処置済みなので除外
            if (dyr < 0 && dx == 1) continue;

            // 必要ジャンプ高度：高低差のみ（下方向は0）
            int requiredHeight = Mathf.Max(0, dyr);
            int requiredWidth = dx;

            if (!HasClearPath(node.Cell, target.Cell, hasTile)) continue;

            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            float cost = dist * _jumpCostMultiplier;
            node.AddEdge(new NavEdge(target, cost, requiredHeight, requiredWidth));
        }
    }

    /// <summary>
    /// 空間チェック：始点と終点で囲まれる矩形内にタイルがないか確認
    /// 高い方のノードと同じX列はチェックから除外する（着地点の真上は問わない）
    /// </summary>
    private bool HasClearPath(Vector2Int from, Vector2Int to, Func<Vector2Int, bool> hasTile)
    {
        int xMin = Mathf.Min(from.x, to.x);
        int xMax = Mathf.Max(from.x, to.x);
        int yMin = Mathf.Min(from.y, to.y);
        int yMax = Mathf.Max(from.y, to.y);

        // 高い方のノードのX列をチェックから除外
        int excludeX = (from.y >= to.y) ? from.x : to.x;

        for (int x = xMin; x <= xMax; x++)
        {
            if (x == excludeX) continue;
            for (int y = yMin; y <= yMax; y++)
                if (hasTile(new Vector2Int(x, y))) return false;
        }

        return true;
    }
}