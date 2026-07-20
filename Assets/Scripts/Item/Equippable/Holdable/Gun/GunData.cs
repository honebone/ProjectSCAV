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
    [SerializeField] private FireType _fireType;
    [SerializeField] private int _burstRounds;
    [SerializeField] private float _burstRate;
    [SerializeField, Header("移動中に適用される拡散ペナルティ")] private float _spreadPenalty_move;
    [SerializeField,Header("一発撃つごとに増加する拡散ペナルティ")] private float _spreadPenalty_fire;
    [SerializeField, Header("射撃拡散ペナルティの最大値")] private float _maxSpreadPenalty_fire;
    [SerializeField, Header("一秒毎の射撃拡散ペナルティの減少量")] private float _spreadLoss;
    [SerializeField] private PjtlData _pjtlData;

    public int MagCap => _magCap;
    public float ReloadTime => _reloadTime;
    public float FireRate => _fireRate;
    public FireType FireType => _fireType;
    public int burstRounds => _burstRounds;
    public float BurstRate => _burstRate;

    public float MaxSpreadPenalty_fire => _maxSpreadPenalty_fire;
    /// <summary>射撃時の拡散ペナルティ 一発撃つごとに追加</summary>
    public float SpreadPenalty_Move => _spreadPenalty_move;
    /// <summary>移動中の拡散ペナルティ</summary>
    public float SpreadPenalty_Fire => _spreadPenalty_fire;
    /// <summary>射撃時の拡散ペナルティ減少量 [度/s]</summary>
    public float SpreadLoss => _spreadLoss;
    public PjtlData PjtlData => _pjtlData;

    public override ItemModel CreateModel() { return new GunModel(this); }
}
public enum FireType { semi, auto, burst }
