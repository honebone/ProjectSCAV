using UnityEngine;

/// <summary>
/// NavGraph のノードとエッジを Scene View に Gizmos で可視化するデバッグ用クラス
/// </summary>
public class NavGraphDebugDrawer : MonoBehaviour
{
    [SerializeField] private NavGraphScanner _scanner;

    [Header("表示切り替え")]
    [SerializeField] private bool _showNodes = true;
    [SerializeField] private bool _showWalk = true;
    [SerializeField] private bool _showFall = true;
    [SerializeField] private bool _showJump = true;

    [Header("ノードサイズ")]
    [SerializeField] private float _nodeRadius = 0.15f;

    private void OnDrawGizmos()
    {
        NavGraph graph = _scanner?.StageContext?.NavGraph;
        if (graph == null) return;

        foreach (NavNode node in graph.Nodes.Values)
        {
            Vector3 fromPos = CellToGizmoPos(node.Cell);

            if (_showNodes)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(fromPos, _nodeRadius);
            }

            foreach (NavEdge edge in node.Edges)
            {
                if (!ShouldDraw(edge.Type)) continue;

                Gizmos.color = EdgeColor(edge.Type);
                Gizmos.DrawLine(fromPos, CellToGizmoPos(edge.To.Cell));
            }
        }
    }

    private bool ShouldDraw(EdgeType type) => type switch
    {
        EdgeType.Walk => _showWalk,
        EdgeType.Fall => _showFall,
        EdgeType.Jump => _showJump,
        _ => false
    };

    private Color EdgeColor(EdgeType type) => type switch
    {
        EdgeType.Walk => Color.green,
        EdgeType.Fall => Color.blue,
        EdgeType.Jump => Color.red,
        _ => Color.white
    };

    /// <summary>
    /// セル座標をGizmos描画用のワールド座標に変換
    /// タイルの中央に描画するため 0.5f オフセット
    /// </summary>
    private Vector3 CellToGizmoPos(Vector2Int cell)
        => new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
}