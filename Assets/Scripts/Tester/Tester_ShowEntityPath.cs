using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester_ShowEntityfPath : MonoBehaviour
{
    [SerializeField] private EntityView _view;

    private void OnDrawGizmos()
    {
        //NavGraph graph = _scanner?.StageContext?.NavGraph;
        NavPath path = _view?.NavPath;
        if (path == null) return;

        for (int i = 0; i < path.Nodes.Count - 1; i++)
        {
            NavNode node = path.Nodes[i];
            Vector3 fromPos = CellToGizmoPos(node.Cell);

            NavEdge edge = node.GetEdge(path.Nodes[i + 1]);
            Gizmos.color = EdgeColor(edge.Type);
            Gizmos.DrawLine(fromPos, CellToGizmoPos(edge.To.Cell));
        }
    }

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
