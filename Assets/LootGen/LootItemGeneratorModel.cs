using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ルートボックス1つ分のアイテム抽選・配置を担当するpure C#クラス
/// LootGeneratorModelから呼び出され、対象のLootbox用のInventoryModelを生成して返す
/// </summary>
public class LootItemGeneratorModel
{
    private readonly List<ItemModel> _allItems;                       // 単価昇順ソート済み
    private readonly Dictionary<ItemTag, List<ItemModel>> _itemsByTag; // タグごとに単価昇順ソート済み

    public LootItemGeneratorModel(ItemDatabaseModel itemDatabase)
    {
        _allItems = BuildAllItems(itemDatabase);
        _itemsByTag = BuildItemsByTag(_allItems);
    }

    // -------------------------------------------------------
    // 前準備
    // -------------------------------------------------------

    private List<ItemModel> BuildAllItems(ItemDatabaseModel itemDatabase)
    {
        List<ItemModel> list = itemDatabase.AllItems.Select(data => data.CreateModel()).ToList();
        list.Sort((a, b) => a.UnitCost.CompareTo(b.UnitCost));
        return list;
    }

    /// <summary>タグにXXを含んでさえいれば(複数タグ保持でも)そのタグのリストに含める</summary>
    private Dictionary<ItemTag, List<ItemModel>> BuildItemsByTag(List<ItemModel> allItemsSorted)
    {
        var dict = new Dictionary<ItemTag, List<ItemModel>>();
        foreach (ItemTag tag in Enum.GetValues(typeof(ItemTag)))
        {
            // allItemsSortedは単価昇順なので、Whereでフィルタしても順序は維持される
            dict[tag] = allItemsSorted.Where(item => item.ItemTags.Contains(tag)).ToList();
        }
        return dict;
    }

    // -------------------------------------------------------
    // 生成本体
    // -------------------------------------------------------

    /// <summary>
    /// ロトボックス1つ分のアイテムを抽選・配置し、InventoryModelを返す
    /// </summary>
    /// <param name="lootbox">タグ候補・includeAllChance・インベントリの高さを参照する対象</param>
    /// <param name="targetCost">このロトボックスに割り当てられたコスト(LootGeneratorModelが算出したlootboxCost)</param>
    public InventoryModel GenerateItems(Lootbox lootbox, float targetCost)
    {
        int width = Constants.Instance.InventorySlotsPerRow;
        int height = lootbox.InventorySize;
        InventoryModel inventory = new InventoryModel(width, height);

        List<ItemModel> pool = SelectPool(lootbox);

        float remainingCost = Extensions.NormalDistribution(
            targetCost,
            targetCost * Constants.Instance.LootCostStdDevRatio);

        DevLog.Log($"[LootItemGeneratorModel] 最終コスト決定: {remainingCost} (目標:{targetCost})");

        while (pool.Count > 0)
        {
            float minUnitCost = pool[0].UnitCost;
            if (remainingCost < minUnitCost) break; // 終了条件：プール内最安値未満

            float roll = UnityEngine.Random.Range(minUnitCost, remainingCost);
            DevLog.Log($"[LootItemGeneratorModel] コストロール: {roll} (残りコスト:{remainingCost})");

            int startIndex = FindCandidateIndex(pool, roll);

            if (!TryPlaceFromIndex(pool, startIndex, inventory, out ItemModel placedItem))
            {
                DevLog.Log("[LootItemGeneratorModel] 配置可能なアイテムが見つからなかったため生成を終了します");
                break;
            }

            int count = DecideCount(placedItem, remainingCost);
            inventory.TryAddAuto(new ItemStackModel(placedItem, count)); // 配置可否はTryPlaceFromIndexで確認済み

            float spent = placedItem.UnitCost * count;
            remainingCost -= spent;

            DevLog.Log($"[LootItemGeneratorModel] 排出: {placedItem.Name} x{count} (消費コスト:{spent}, 残りコスト:{remainingCost})");
        }

        DevLog.Log($"[LootItemGeneratorModel] 生成完了 余りコスト:{remainingCost}");

        return inventory;
    }

    // -------------------------------------------------------
    // プール決定
    // -------------------------------------------------------

    private List<ItemModel> SelectPool(Lootbox lootbox)
    {
        if (lootbox.IncludeAllChance.Dice())
        {
            DevLog.Log("[LootItemGeneratorModel] タグ抽選結果: 全アイテム対象");
            return new List<ItemModel>(_allItems);
        }

        ItemTag selectedTag = SelectTag(lootbox.TagCandidates);
        DevLog.Log($"[LootItemGeneratorModel] タグ抽選結果: {selectedTag}");
        return new List<ItemModel>(_itemsByTag[selectedTag]);
    }

    private ItemTag SelectTag(IReadOnlyList<ItemTagCandidate> candidates)
    {
        List<int> weights = candidates.Select(c => c.Weight).ToList();
        int index = weights.ChoiceWithWeight();
        return candidates[index].ItemTag;
    }

    // -------------------------------------------------------
    // アイテム選択・配置
    // -------------------------------------------------------

    /// <summary>単価がroll以下となる最後（最大）のインデックスを返す</summary>
    private int FindCandidateIndex(List<ItemModel> pool, float roll)
    {
        int index = 0;
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].UnitCost > roll) break;
            index = i;
        }
        return index;
    }

    /// <summary>
    /// startIndexから単価が大きい方向へ順に配置を試す
    /// 配置できた場合はtrueを返し、placedItemに結果を格納する
    /// </summary>
    private bool TryPlaceFromIndex(List<ItemModel> pool, int startIndex, InventoryModel inventory, out ItemModel placedItem)
    {
        for (int i = startIndex; i < pool.Count; i++)
        {
            ItemModel candidate = pool[i];
            bool canPlace = inventory.CanFitAnywhere(candidate.Size);
            DevLog.Log($"[LootItemGeneratorModel] 候補: {candidate.Name} (単価:{candidate.UnitCost}) 配置可能:{canPlace}");

            if (canPlace)
            {
                placedItem = candidate;
                return true;
            }
        }

        placedItem = null;
        return false;
    }

    // -------------------------------------------------------
    // 個数決定
    // -------------------------------------------------------

    private int DecideCount(ItemModel item, float remainingCost)
    {
        float mean = item.MaxStack * Constants.Instance.LootItemCountMeanRatio;
        float stdDev = mean * Constants.Instance.LootItemCountStdDevRatio;

        int count = Mathf.RoundToInt(Extensions.NormalDistribution(mean, stdDev));
        count = Mathf.Max(count, 1);
        count = Mathf.Min(count, item.MaxStack);

        // 残りコストを超過しないように丸める
        int affordable = Mathf.FloorToInt(remainingCost / item.UnitCost);
        count = Mathf.Min(count, affordable);
        count = Mathf.Max(count, 1); // 選択時点でUnitCost<=remainingCostは保証済みなので理論上1以上

        return count;
    }
}