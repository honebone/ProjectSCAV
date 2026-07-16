using System.Collections.Generic;
using System.Linq;

/// <summary>
/// アイテムデータの検索APIを提供する pure C# クラス
/// key は Addressables のアドレス文字列（ItemDatabaseLoader が構築する）
/// UnityにもAddressablesにも依存しないため、Model層の方針に沿う
/// </summary>
public class ItemDatabaseModel
{
    private readonly Dictionary<string, ItemData> _items;
    private readonly List<ItemData> _allItems;

    /// <summary>登録されている全アイテムのリスト</summary>
    public IReadOnlyList<ItemData> AllItems => _allItems;

    public ItemDatabaseModel(Dictionary<string, ItemData> items)
    {
        _items = items;
        _allItems = _items.Values.ToList();
    }

    /// <summary>Addressableアドレス(=旧IDに相当)からアイテムを取得する</summary>
    public ItemData Get(string id)
    {
        if (!_items.TryGetValue(id, out ItemData data))
        {
            DevLog.Error($"[ItemDatabaseModel] IDが見つかりませんでした: {id}");
            return null;
        }
        return data;
    }

    /// <summary>指定したタグのいずれかを持つアイテムをすべて返す</summary>
    public List<ItemData> GetByTags(List<ItemTag> tags)
    {
        return _items.Values.Where(item => item.ItemTags.Any(tags.Contains)).ToList();
    }
}