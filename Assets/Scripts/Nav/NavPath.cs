using System.Collections.Generic;

/// <summary>
/// A*経路探索の結果を保持するデータクラス
/// </summary>
public class NavPath
{
    private readonly List<NavNode> _nodes;
    private readonly bool _isSameGround;

    public IReadOnlyList<NavNode> Nodes => _nodes;

    /// <summary>
    /// 足場間の移動が必要な経路が存在するか
    /// 同一足場上にいる場合・経路がない場合はともにfalse
    /// </summary>
    public bool IsReachable => _nodes.Count > 0;

    /// <summary>
    /// 自身と目標が同じ足場上にあるか
    /// IsReachable == false かつ IsSameGround == true なら「同一足場」
    /// IsReachable == false かつ IsSameGround == false なら「到達不能」
    /// </summary>
    public bool IsSameGround => _isSameGround;

    private NavPath(List<NavNode> nodes, bool isSameGround)
    {
        _nodes = nodes;
        _isSameGround = isSameGround;
    }

    public NavPath(List<NavNode> nodes) : this(nodes, false) { }

    /// <summary>経路なし（到達不能）を表すインスタンスを返す</summary>
    public static NavPath Unreachable => new NavPath(new List<NavNode>(), false);

    /// <summary>自身と目標が同一足場上にあることを表すインスタンスを返す</summary>
    public static NavPath OnSameGround => new NavPath(new List<NavNode>(), true);
}