using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートボックスの生成ロジックを担う pure C# クラス
/// マーカーの抽選・ボックスのインスタンス化・アイテム生成をすべて担当する
/// </summary>
public class LootGeneratorModel
{
    private readonly LootItemGeneratorModel _itemGenerator;
    private readonly ILootboxSpawner _spawner;

    public LootGeneratorModel(LootItemGeneratorModel itemGenerator, ILootboxSpawner spawner)
    {
        _itemGenerator = itemGenerator;
        _spawner = spawner;
    }

    /// <summary>
    /// 生成候補地点のリストを受け取り、合計コストに達するまでルートボックスを生成する
    /// 全候補地点が選ばれた場合も終了する
    /// </summary>
    /// <param name="spawnPoints">全部屋から収集した生成候補地点のリスト</param>
    /// <param name="totalCost">エリア全体の合計コスト</param>
    /// <param name="baseCostPerLootbox">ルートボックス1つあたりの基本コスト</param>
    public void GenerateLoot(
        IReadOnlyList<ILootboxSpawnPoint> spawnPoints,
        int totalCost,
        int baseCostPerLootbox)
    {
        // 未選択マーカーのプール（重複なし抽選のためコピーを作成）
        List<ILootboxSpawnPoint> pool = new List<ILootboxSpawnPoint>(spawnPoints);

        int accumulatedCost = 0;

        while (accumulatedCost < totalCost && pool.Count > 0)
        {
            // マーカーを均等抽選
            int index = Random.Range(0, pool.Count);
            ILootboxSpawnPoint marker = pool[index];
            pool.RemoveAt(index);

            // マーカーが持つ候補からルートボックスを重みづけ抽選
            // 重み = コスト倍率の逆数（安いボックスほど出やすい）
            LootboxCandidate selected = SelectCandidate(marker.Candidates);

            // コストを加算
            int lootboxCost = Mathf.RoundToInt(baseCostPerLootbox * selected.CostMultiplier);
            accumulatedCost += lootboxCost;

            // ルートボックスをインスタンス化
            Lootbox lootbox = _spawner.Spawn(selected.Prefab, marker.Position);
            if (lootbox == null)
            {
                DevLog.Error("[LootGeneratorModel] Spawn が null を返しました");
                continue;
            }

            InventoryModel loot = _itemGenerator.GenerateItems(selected.Data, lootboxCost);
            lootbox.Init(loot);
        }

        DevLog.Log($"[LootGeneratorModel] 生成完了 累計コスト:{accumulatedCost} / 目標:{totalCost}");
    }

    /// <summary>
    /// 候補リストからコスト倍率の逆数を重みとして1つ選ぶ
    /// </summary>
    private LootboxCandidate SelectCandidate(IReadOnlyList<LootboxCandidate> candidates)
    {
        // 重み = 1 / CostMultiplier
        List<float> weights = new List<float>(candidates.Count);
        foreach (LootboxCandidate c in candidates)
        {
            weights.Add(1f / Mathf.Max(c.CostMultiplier, 0.0001f)); // 0除算を防ぐ
        }

        int selectedIndex = weights.ChoiceWithWeight();
        return candidates[selectedIndex];
    }
}