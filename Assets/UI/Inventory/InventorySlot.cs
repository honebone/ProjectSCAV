using UnityEngine;

/// <summary>
/// インベントリのグリッド1マス分のUI。
/// アイテムアイコン自体は持たず、位置情報の提供とスロット背景の表示のみを担う。
/// </summary>
public class InventorySlot : UIPanel
{
    private Vector2Int _gridPos;
    private RectTransform _rectTransform;

    public Vector2Int GridPos => _gridPos;

    /// <summary>このスロットを基準としたアイコンの配置位置（左上原点）</summary>
    public Vector2 AnchoredPosition => _rectTransform.anchoredPosition;

    public void Init(Vector2Int pos, Vector2 cellSize, Vector2 spacing)
    {
        _gridPos = pos;
        _rectTransform = (RectTransform)transform;
    }
}
