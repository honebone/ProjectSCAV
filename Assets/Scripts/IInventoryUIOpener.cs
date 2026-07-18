/// <summary>
/// 画面左のInventoryViewを開く処理を抽象化するインターフェース
/// LootboxPresenter等がこれ越しにUIを開く（直接参照を持たせないため）
/// </summary>
public interface IInventoryUIOpener
{
    void Open(InventoryModel inventory);
}