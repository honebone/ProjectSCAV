using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private InventorySlot _slotPrefab;
    [SerializeField] private ItemStackIcon _iconPrefab;
    [SerializeField] private RectTransform _gridRoot;
    [SerializeField] private RectTransform _iconRoot; // _gridRootと同じ範囲に重ねて配置し、手前に表示する
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private Vector2 _spacing;

    private InventorySlot[,] _slots;

    // origin（左上座標） → 生成中のアイコン
    private readonly Dictionary<Vector2Int, ItemStackIcon> _icons = new();

    // Presenterが購読するイベント
    public event Action<Vector2Int, Vector2Int> OnSwapRequested;
    public event Action<Vector2Int> OnDropOutsideRequested;

    public void SetSlot(int width, int height)
    {
        for (int i = 0; i < _gridRoot.childCount; i++) { Destroy(_gridRoot.GetChild(i).gameObject); }
        for (int i = 0; i < _iconRoot.childCount; i++) { Destroy(_iconRoot.GetChild(i).gameObject); }
        _icons.Clear();

        _slots = new InventorySlot[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                InventorySlot slot = Instantiate(_slotPrefab, _gridRoot);
                slot.Init(new Vector2Int(x, y), _cellSize, _spacing);
                _slots[x, y] = slot;
            }
        }
    }

    /// <summary>Presenterから呼ばれる。変更のあったスロットだけ更新する</summary>
    public void UpdateSlot(Vector2Int origin, ItemStackModel stack)
    {
        // 既存のアイコンがあれば削除（更新・削除どちらでも一旦消す）
        if (_icons.TryGetValue(origin, out ItemStackIcon existing))
        {
            Destroy(existing.gameObject);
            _icons.Remove(origin);
        }

        if (stack == null) return;

        // 新規生成
        ItemStackIcon icon = Instantiate(_iconPrefab, _iconRoot);
        //Vector2 pos = _slots[origin.x, origin.y].AnchoredPosition;
        Vector2 pos = new Vector2(
            origin.x * (_cellSize.x + _spacing.x),
            -origin.y * (_cellSize.y + _spacing.y)
        );
        icon.Init(stack, _cellSize, _spacing, pos);
        _icons[origin] = icon;
    }
}