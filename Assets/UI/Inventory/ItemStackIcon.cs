using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// アイテムアイコン。InventoryViewの_iconRoot直下に生成される。
/// 自身のサイズ・位置をアイテムのSizeとセル情報から計算して設定する。
/// </summary>
public class ItemStackIcon : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _slotImage;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    /// <summary>
    /// アイコンの内容・サイズ・位置を設定する
    /// </summary>
    /// <param name="stack">アイテム</param>
    /// <param name="cellSize">1マスあたりの辺の長さ</param>
    /// <param name="spacing">スロット同士の間隔</param>
    /// <param name="anchoredPosition">左上スロットを基準としたアイコンのアンカー位置</param>
    public void Init(ItemStackModel stack, Vector2 cellSize, Vector2 spacing, Vector2 anchoredPosition)
    {
        Vector2Int size = stack.Size;

        // _slotImage：spacingを考慮したサイズ（占有マス全体を覆う大きさ）
        Vector2 slotSize = new Vector2(
            size.x * cellSize.x + (size.x - 1) * spacing.x,
            size.y * cellSize.y + (size.y - 1) * spacing.y
        );
        _slotImage.rectTransform.sizeDelta = slotSize;

        // _itemIcon：spacingを考慮しないサイズ（アイテム本来の見た目のサイズ）
        Vector2 iconSize = new Vector2(
            size.x * cellSize.x,
            size.y * cellSize.y
        );
        _itemIcon.rectTransform.sizeDelta = iconSize;

        // アイコン全体の位置（左上スロット基準）
        _rectTransform.anchoredPosition = anchoredPosition;

        _itemIcon.sprite = stack.Item.Data.Sprite;

        if (stack.Item.MaxStack > 1)
        {
            _amountText.gameObject.SetActive(true);
            _amountText.text = stack.Amount.ToString();
        }
        else
        {
            _amountText.gameObject.SetActive(false);
        }
    }
}
