using System.Collections.Generic;
using UnityEngine;

// -------------------------------------------------------
// 手に持てるアイテムの基底
// -------------------------------------------------------

/// <summary>
/// 手に持てるアイテムの基底ScriptableObject
/// 持ち替え時間を持つ
/// </summary>
public abstract class HoldableItemData : EquippableItemData
{
    [SerializeField] private float _drawTime;
    [SerializeField] private GameObject _visualPrefab;
    [SerializeField] private float _useTime;

    /// <summary>持ち替えにかかる時間（秒）</summary>
    public float DrawTime => _drawTime;
    public GameObject VisualPrefab => _visualPrefab;
    public float UseTime => _useTime;
}
