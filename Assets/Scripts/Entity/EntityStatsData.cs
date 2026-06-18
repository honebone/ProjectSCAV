using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンティティ固有のステータス初期値を保持するScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Entity/StatsData", fileName = "EntityStatsData")]
public class EntityStatsData : ScriptableObject
{
    [Header("HP / Armor")]
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _maxArmor = 50f;
    [SerializeField] private float _armorRegenDelay = 3f;  // アーマー再生が始まるまでの秒数
    [SerializeField] private float _armorRegenSpeed = 5f;  // アーマー毎秒回復量

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector2 _jumpPower = new Vector2(3f, 6f); // x:水平, y:垂直

    [Header("Sight")]
    [SerializeField] private float _fovAngle = 30f;
    [SerializeField] private float _sightRange;

    [Header("Faction")]
    [SerializeField] private Faction _faction;//自身の派閥
    [SerializeField] private List<Faction> _hostiles;//敵対する派閥

    [Header("EquipmentSlot")]
    [SerializeField] private int _gunSlot;
    [SerializeField] private int _gearSlot;
    [SerializeField] private List<ImplantPart> _implantSlot = new List<ImplantPart> { ImplantPart.Head, ImplantPart.Body, ImplantPart.Arm, ImplantPart.Leg };

    [Header("Heat（プレイヤー専用。敵には不要）")]
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _heatRecovery = 10f;

    public float MaxHp => _maxHp;
    public float MaxArmor => _maxArmor;
    public float ArmorRegenDelay => _armorRegenDelay;
    public float ArmorRegenSpeed => _armorRegenSpeed;
    public float MoveSpeed => _moveSpeed;
    public Vector2 JumpPower => _jumpPower;
    public float FOVAngle => _fovAngle;
    public float SightRange => _sightRange;
    public float MaxHeat => _maxHeat;
    public float HeatRecovery => _heatRecovery;
    public Faction Faction => _faction;
    public List<Faction> Hostiles => _hostiles;
    public int GunSlot => _gunSlot;
    public int GearSlot => _gearSlot;
    public IReadOnlyList<ImplantPart>ImplantSlot => _implantSlot;

    public virtual EntityModel CreateModel(EntityView view) { return new EntityModel(this, view); }
}
