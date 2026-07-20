using UnityEngine;

/// <summary>
/// ルートボックスのインスタンス化を担うインターフェース
/// LootGeneratorModel はこのインターフェース越しに生成を依頼し、MonoBehaviourに直接依存しない
/// </summary>
public interface ILootboxSpawner
{
    /// <summary>
    /// 指定したプレハブを指定した座標にインスタンス化し、Lootboxコンポーネントを返す
    /// </summary>
    LootboxPresenter SpawnLootbox(GameObject prefab, Vector2 position);
}