using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 銃のScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Item/GunData")]
public class GunData : HoldableItemData
{
    [SerializeField] private int _magCap;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _maxSpread;
    [SerializeField] private float _spreadPenalty_move;
    [SerializeField] private float _spreadPenalty_fire;
    [SerializeField] private float _spreadLoss;
    [SerializeField] private PjtlData _pjtlData;

    public int MagCap => _magCap;
    public float ReloadTime => _reloadTime;
    public float FireRate => _fireRate;

    public float MaxSpread => _maxSpread;
    /// <summary>射撃時の拡散ペナルティ 一発撃つごとに追加</summary>
    public float SpreadPenalty_Move => _spreadPenalty_move;
    /// <summary>移動中の拡散ペナルティ</summary>
    public float SpreadPenalty_Fire => _spreadPenalty_fire;
    /// <summary>射撃時の拡散ペナルティ減少量 [度/s]</summary>
    public float SpreadLoss => _spreadLoss;
    public PjtlData PjtlData => _pjtlData;

    public override ItemModel CreateModel() { return new GunModel(this); }
}
