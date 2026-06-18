using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ナビゲーショングラフのコンテナ
/// ノードの追加・取得・削除（差分更新用）を提供する
/// </summary>
public class NavGraph
{
    private readonly Dictionary<Vector2Int, NavNode> _nodes = new();

    public IReadOnlyDictionary<Vector2Int, NavNode> Nodes => _nodes;

    /// <summary>指定セルのノードを返す。存在しなければ新規作成して返す</summary>
    public NavNode GetOrCreate(Vector2Int cell)
    {
        if (!_nodes.TryGetValue(cell, out NavNode node))
        {
            node = new NavNode(cell);
            _nodes[cell] = node;
        }
        return node;
    }

    /// <summary>指定セルのノードを返す。存在しなければ null を返す</summary>
    public NavNode Get(Vector2Int cell)
    {
        _nodes.TryGetValue(cell, out NavNode node);
        return node;
    }

    /// <summary>cell番地にノードがあるか返す　あるならnodeにそのノードを代入 </summary>
    public bool TryGetNode(Vector2Int cell, out NavNode node)
        => _nodes.TryGetValue(cell, out node);

    ///// <summary>
    ///// 差分更新用：指定セルのノードと、
    ///// そのノードへ向かう他ノードのエッジを削除する
    ///// </summary>
    //public void Remove(Vector2Int cell)
    //{
    //    if (!_nodes.TryGetValue(cell, out NavNode node)) return;

    //    foreach (NavNode n in _nodes.Values)
    //        n.Edges.RemoveAll(e => e.To == node);

    //    _nodes.Remove(cell);
    //}
}