using UnityEngine;

/// <summary>
/// Lootboxのインタラクト処理を担うPresenter
/// ハイライト切り替えと、開封時のInventoryUI表示要求を行う
/// </summary>
public class LootboxPresenter : MonoBehaviour, IInteractable
{
    [SerializeField] private Lootbox _lootbox;
    [SerializeField] private Renderer _renderer; // ハイライト用マテリアルを持つRenderer

    private static readonly int ActiveProp = Shader.PropertyToID("Active");

    private IInventoryUIOpener _inventoryUIOpener;

    /// <summary>外部（AreaManager等）から注入する</summary>
    public void Init(IInventoryUIOpener inventoryUIOpener)
    {
        _inventoryUIOpener = inventoryUIOpener;
    }

    public void SetHighlighted(bool active)
    {
        _renderer.material.SetFloat(ActiveProp, active ? 1f : 0f);
    }

    public void Interact()
    {
        _inventoryUIOpener?.Open(_lootbox.Loot);
    }
}