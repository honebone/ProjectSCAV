using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートボックスの生成候補地点を示すマーカー
/// 部屋プレハブの子オブジェクトとして配置し、Room が GetComponentsInChildren で収集する
/// </summary>
public class LootboxMarker : MonoBehaviour, ILootboxSpawnPoint
{
    [SerializeField] private List<LootboxCandidate> _candidates;

    public IReadOnlyList<LootboxCandidate> Candidates => _candidates;
    public Vector2 Position => transform.position;
}
