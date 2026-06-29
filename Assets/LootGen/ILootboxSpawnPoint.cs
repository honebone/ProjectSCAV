using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートボックス生成候補地点のデータ
/// LootboxMarkerのインスペクターから設定する
/// </summary>
[System.Serializable]
public struct LootboxCandidate
{
    [SerializeField] private GameObject _prefab;
    //[SerializeField] private float _costMultiplier;

    public GameObject Prefab => _prefab;
    public float CostMultiplier => _prefab.GetComponent<Lootbox>().CostMultiplier;
}

/// <summary>
/// ルートボックスの生成候補地点を表すインターフェース
/// LootGeneratorModel はこのインターフェース越しに情報を受け取り、MonoBehaviourに直接依存しない
/// </summary>
public interface ILootboxSpawnPoint
{
    IReadOnlyList<LootboxCandidate> Candidates { get; }
    Vector2 Position { get; }
}