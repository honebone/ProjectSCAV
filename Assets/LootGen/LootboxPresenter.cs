using UnityEngine;

/// <summary>
/// Lootboxのインタラクト処理を担うPresenter
/// ハイライト切り替えと、開封時のInventoryUI表示要求を行う
/// </summary>
public class LootboxPresenter : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _renderer; // ハイライト用マテリアルを持つRenderer

    private static readonly int ActiveProp = Shader.PropertyToID("Active");

    private LootboxModel _model;
    private IInventoryUIOpener _inventoryUIOpener;

    /// <summary>外部（AreaManager等）から注入する</summary>
    public void Init(InventoryModel inventoryModel, IInventoryUIOpener inventoryUIOpener)
    {
        _inventoryUIOpener = inventoryUIOpener;
        _model = new LootboxModel(inventoryModel);
    }

    public void SetHighlighted(bool active)
    {
        _renderer.material.SetInt(ActiveProp, active ? 1 : 0);
    }

    public void Interact()
    {
        _inventoryUIOpener?.OpenInventoryUI(_model.Loot);
    }
}