/// <summary>
/// ナビゲーショングラフの辺
/// 移動先ノード・移動タイプ・コスト・ジャンプ要求値を保持する
/// </summary>
public enum EdgeType { Walk, Jump, Fall }

public class NavEdge
{
    private readonly NavNode _to;
    private readonly EdgeType _type;
    private readonly float _cost;
    private readonly int _requiredJumpHeight; // Jumpのみ有効（タイル数）
    private readonly int _requiredJumpWidth;  // Jumpのみ有効（タイル数）

    public NavNode To => _to;
    public EdgeType Type => _type;
    public float Cost => _cost;
    public int RequiredJumpHeight => _requiredJumpHeight;
    public int RequiredJumpWidth => _requiredJumpWidth;

    /// <summary>Walk / Fall 用コンストラクタ</summary>
    public NavEdge(NavNode to, EdgeType type, float cost)
    {
        _to = to;
        _type = type;
        _cost = cost;
        _requiredJumpHeight = 0;
        _requiredJumpWidth = 0;
    }

    /// <summary>Jump 用コンストラクタ</summary>
    public NavEdge(NavNode to, float cost, int requiredJumpHeight, int requiredJumpWidth)
    {
        _to = to;
        _type = EdgeType.Jump;
        _cost = cost;
        _requiredJumpHeight = requiredJumpHeight;
        _requiredJumpWidth = requiredJumpWidth;
    }

    public string GetInfo()
    {
        return $"to({To.Cell} by {_type})";
    }
}