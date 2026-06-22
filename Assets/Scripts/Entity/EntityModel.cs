using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class EntityModel
{
    private readonly IProjectileSpawner _projectileSpawner;
    public EntityStats Stats { get; }
    private Faction _faction;
    public Faction Faction => _faction;

    private List<Faction> _hostiles;
    public IReadOnlyList<Faction> Hostiles => _hostiles;

    // -------------------------------------------------------
    // 現在値（StatValueで管理する最大値と別に保持する）
    // -------------------------------------------------------

    private int _hp;
    private int _armor;

    public int Hp => _hp;
    public int Armor => _armor;
    public bool Alive => _hp > 0;

    // アーマー再生タイマー（最後にダメージを受けてからの経過時間）
    private float _armorRegenTimer;

    private Vector2 _position;
    public Vector2 Position => _position;
    //public IProjectileSpawner ProjectileSpawner => _projectileSpawner;

    public event Action<int> OnArmorDamaged;
    public event Action OnArmorBreak;
    public event Action<int> OnHPDamaged;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------

    public EntityModel(EntityStatsData data,IProjectileSpawner projectileSpawner)
    {
        Stats = new EntityStats(data);
        _hp = Stats.MaxHp.IntValue;
        _armor = Stats.MaxArmor.IntValue;
        _faction = data.Faction;
        _hostiles = data.Hostiles;
        _projectileSpawner = projectileSpawner;
    }

    public virtual void Tick(float deltaTime,Vector2 position)
    {
        _position = position;
    }

    // -------------------------------------------------------
    // HP / Armor
    // -------------------------------------------------------

    /// <summary>ダメージを受ける。Armorを先に削り、残りをHPへ</summary>
    public void Damage(float damage)
    {
        int remain = damage.ToInt();
        if (remain == 0) return;

        if (_armor > 0)
        {
            int armorDMG = _armor > remain ? remain : _armor;
            _armor -= armorDMG;
            remain -= armorDMG;
            DevLog.Log($"ArmorDamage {armorDMG}");
            OnArmorDamaged?.Invoke(armorDMG);
            if (_armor == 0)
            {
                DevLog.Log("ArmorBreak");
                OnArmorBreak?.Invoke();
            }
        }

        if(remain > 0)
        {
            int hpDMG = _hp > remain ? remain : _hp;
            _hp -= hpDMG;
            DevLog.Log($"HPDamage {hpDMG}");
            OnHPDamaged?.Invoke(hpDMG);

            if (_hp < 0)
            {
                Die();
            }
        }
    }

    /// <summary>回復する</summary>
    public void Heal(float amount) { }

    /// <summary>アーマー再生処理。毎フレームEntityのUpdateから呼ぶ</summary>
    protected void TickArmorRegen(float deltaTime) { }

    /// <summary>死亡処理</summary>
    protected virtual void Die()
    {
        _hp = 0;
    }
}
