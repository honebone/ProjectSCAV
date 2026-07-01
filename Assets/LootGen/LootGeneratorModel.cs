using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートボックスの生成ロジックを担う pure C# クラス
/// マーカーの抽選・ボックスのインスタンス化・アイテム生成をすべて担当する
/// </summary>
public class LootGeneratorModel
{
    private readonly ItemDatabase _itemDatabase;
    private readonly int _totalCost;
    private readonly int _baseCostPerLootbox;

    public LootGeneratorModel()
    {
        _itemDatabase = DatabaseLocator.Instance.ItemDatabase;
    }

    /// <summary>
    /// 生成候補地点のリストを受け取り、合計コストに達するまでルートボックスを生成する
    /// 全候補地点が選ばれた場合も終了する
    /// </summary>
    /// <param name="spawnPoints">全部屋から収集した生成候補地点のリスト</param>
    /// <param name="totalCost">エリア全体の合計コスト</param>
    public void GenerateLoot(IReadOnlyList<ILootboxSpawnPoint> spawnPoints, int totalCost, int baseCostPerLootbox)
    {
        // TODO: アイテム生成仕様確定後に実装する
    }
}