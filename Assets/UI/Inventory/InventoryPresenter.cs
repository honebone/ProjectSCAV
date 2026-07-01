using UnityEngine;

public class InventoryPresenter : UIPanel
{
    private InventoryModel _model;
    [SerializeField] private InventoryView _view;

    ///// <summary>プレイヤーインベントリなど、参照が固定の場合はInitで一度だけ呼ぶ</summary>
    //public void Init(InventoryModel model)
    //{
    //    _view.SetSlot(model.Width, model.Height);
    //    Open(model);
    //}

    public void Open(InventoryModel model)
    {
        // 前のModelの購読を解除
        if (_model != null) _model.OnItemChanged -= OnItemChanged;

        _view.SetSlot(model.Width, model.Height);
        _model = model;
        _model.OnItemChanged += OnItemChanged;
        RefreshAll();
        SetPanelActive(true);
    }

    public void Close()
    {
        if (_model != null) _model.OnItemChanged -= OnItemChanged;
        SetPanelActive(false);
    }

    private void RefreshAll()
    {
        foreach (var kv in _model.Items)
            _view.UpdateSlot(kv.Key, kv.Value);
    }

    private void OnItemChanged(Vector2Int origin, ItemStackModel stack)
        => _view.UpdateSlot(origin, stack);
}
