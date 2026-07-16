using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/// <summary>
/// Addressables から "Item" ラベルの付いた ItemData（派生型含む）をすべて読み込み、
/// ItemDatabaseModel を構築する
/// Addressables依存をここに閉じ込め、ItemDatabaseModel自体はpure C#に保つ
/// </summary>
public static class ItemDatabaseLoader
{
    private const string ItemLabel = "Item";

    public static async Task<ItemDatabaseModel> LoadAsync()
    {
        // "Item"ラベルが付いた ItemData(またはその派生型)アセットの所在情報を取得
        IList<IResourceLocation> locations =
            await Addressables.LoadResourceLocationsAsync(ItemLabel, typeof(ItemData)).Task;

        Dictionary<string, ItemData> items = new Dictionary<string, ItemData>();

        foreach (IResourceLocation location in locations)
        {
            ItemData data = await Addressables.LoadAssetAsync<ItemData>(location).Task;
            if (data == null)
            {
                DevLog.Warning($"[ItemDatabaseLoader] ロードに失敗しました: {location.PrimaryKey}");
                continue;
            }

            // PrimaryKey = Addressableのアドレス文字列。これをIDとして流用する
            if (items.ContainsKey(location.PrimaryKey))
            {
                DevLog.Warning($"[ItemDatabaseLoader] IDが重複しています: {location.PrimaryKey}");
                continue;
            }

            items[location.PrimaryKey] = data;
        }

        DevLog.Log($"[ItemDatabaseLoader] {items.Count} 件のアイテムをロードしました");
        return new ItemDatabaseModel(items);
    }
}