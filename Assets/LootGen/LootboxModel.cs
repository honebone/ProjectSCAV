using System.Collections.Generic;

public class LootboxModel
{
    private readonly InventoryModel _loot;
    public InventoryModel Loot => _loot;
    public LootboxModel(InventoryModel loot)
    {
        _loot = loot;
    }
}
