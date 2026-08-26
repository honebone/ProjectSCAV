using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModel
{
    private readonly IProjectileSpawner _projectileSpawner;
    public EntityStats Stats { get; }
    private Faction _faction;
    public Faction Faction => _faction;

    private List<Faction> _hostiles;
    public IReadOnlyList<Faction> Hostiles => _hostiles;
    public bool IsHostile(EntityModel target) => _hostiles.Contains(target.Faction);  

    // -------------------------------------------------------
    // 現在値（StatValueで管理する最大値と別に保持する）
    // -------------------------------------------------------

    private int _hp;
    private int _armor;

    public int Hp => _hp;
    public int Armor => _armor;
    public bool Alive => _hp > 0;

    // アーマー再生タイマー（最後にダメージを受けてからの経過時間）
    private float _shieldRegenTimer;

    private Vector2 _position;
    public Vector2 Position => _position;

    // -------------------------------------------------------
    // バフ・デバフ
    // -------------------------------------------------------

    private readonly List<BuffModel> _buffs = new List<BuffModel>();
    public IReadOnlyList<BuffModel> Buffs => _buffs;

    public struct OnDamagedContext
    {
        public EntityModel Source;
        public int HPDamage;
        public int ShieldDamage;
        public int TotalDamage => HPDamage + ShieldDamage;

        public OnDamagedContext(
            EntityModel source,
            int hpDamage,
            int shieldDamage
            )
        {
            Source = source;
            HPDamage = hpDamage;
            ShieldDamage = shieldDamage;
        }
    }

    public event Action<OnDamagedContext> OnDamaged;
    public event Action<int> OnArmorDamaged;
    public event Action OnShieldBreak;
    public event Action<int> OnHPDamaged;
    public event Action<int> OnShieldHealed;
    public event Action<int> OnHPHealed;
    public event Action<BuffModel> OnBuffApplied;
    public event Action<BuffModel> OnBuffRemoved;

    /// <summary>このエンティティが他のエンティティを撃破したとき（victim=倒した相手）</summary>
    public event Action<EntityModel> OnKill;
    /// <summary>このエンティティが死亡したとき</summary>
    public event Action OnDeath;

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
        TickBuffs(deltaTime);
    }

    // -------------------------------------------------------
    // 作用（EffectAction）の解決
    // -------------------------------------------------------

    /// <summary>
    /// 自身に対して働く作用を解決する。ダメージ・回復・バフ付与はすべてこの窓口を経由する
    /// （弾丸命中・パッシブ効果によるトリガーなど、発生源を問わない）
    /// </summary>
    public void Resolve(EffectAction action)
    {
        if (action.DamageAmount != 0f) ResolveDamage(action);
        if (action.HealAmount != 0f) ResolveHeal(action);
        for (int i = 0; i < action.Buffs.Count; i++) ResolveBuff(action.Buffs[i], action.Source);
    }

    /// <summary>パッシブ効果などから投射物を生成する</summary>
    public void SpawnProjectile(FireParams fireParams)
    {
        _projectileSpawner?.SpawnProjectile(fireParams);
    }

    // -------------------------------------------------------
    // HP / Armor
    // -------------------------------------------------------

    private void ResolveDamage(EffectAction action)
    {
        int remain = action.DamageAmount.ToInt();
        if (remain <= 0) return;

        int hpDamage = 0;
        int shieldDamage = 0;
        switch (action.DamageTarget)
        {
            case DamageTarget.ArmorOnly:
                shieldDamage = DamageShield(remain);
                break;
            case DamageTarget.HPOnly:
                hpDamage= DamageHP(remain, action.Source);
                break;
            case DamageTarget.HPAndArmor:
            default:
                shieldDamage = DamageShield(remain);
                remain -= shieldDamage;
                if (remain > 0) hpDamage = DamageHP(remain, action.Source);
                break;
        }

        OnDamagedContext context = new OnDamagedContext(
            action.Source,
            hpDamage,
            shieldDamage
            );

        OnDamaged?.Invoke(context);
    }

    /// <summary>Shieldにダメージを与え、実際に減少した分を返す</summary>
    private int DamageShield(int amount)
    {
        if (_armor <= 0) return 0;

        int armorDMG = _armor > amount ? amount : _armor;
        _armor -= armorDMG;
        OnArmorDamaged?.Invoke(armorDMG);
        if (_armor == 0) OnShieldBreak?.Invoke();

        return armorDMG;
    }
    /// <summary>HPにダメージを与え、実際に減少した分を返す</summary>
    private int DamageHP(int amount, EntityModel source)
    {
        if (amount <= 0 || _hp <= 0) return 0;

        int hpDMG = _hp > amount ? amount : _hp;
        _hp -= hpDMG;
        OnHPDamaged?.Invoke(hpDMG);

        if (_hp <= 0)
        {
            Die();
            OnDeath?.Invoke();
            source?.OnKill?.Invoke(this);
        }

        return hpDMG;
    }

    private void ResolveHeal(EffectAction action)
    {
        int amount = action.HealAmount.ToInt();
        if (amount <= 0) return;

        switch (action.HealTarget)
        {
            case HealTarget.HPOnly:
                HealHP(amount);
                break;
            case HealTarget.ArmorOnly:
                HealArmor(amount);
                break;
            case HealTarget.HPAndArmor:
            default:
                HealHP(amount);
                HealArmor(amount);
                break;
        }
    }

    private void HealHP(int amount)
    {
        if (!Alive) return;

        int healed = Mathf.Min(amount, Stats.MaxHp.IntValue - _hp);
        if (healed <= 0) return;

        _hp += healed;
        OnHPHealed?.Invoke(healed);
    }

    private void HealArmor(int amount)
    {
        int healed = Mathf.Min(amount, Stats.MaxArmor.IntValue - _armor);
        if (healed <= 0) return;

        _armor += healed;
        OnShieldHealed?.Invoke(healed);
    }

    /// <summary>アーマー再生処理。毎フレームEntityのUpdateから呼ぶ</summary>
    protected void TickArmorRegen(float deltaTime) { }

    /// <summary>死亡処理</summary>
    protected virtual void Die()
    {
        _hp = 0;
        OnDeath?.Invoke();
    }

    // -------------------------------------------------------
    // バフ・デバフ
    // -------------------------------------------------------

    private void ResolveBuff(BuffApplication application, EntityModel source)
    {
        BuffData data = application.Buff;
        if (data == null) return;

        BuffModel existing = _buffs.Find(b => b.BuffId == data.BuffId);
        if (existing == null)// 同名のバフが既にあるなら
        {
            BuffModel buff = data.CreateModel(source, application.Duration, application.Stacks);
            buff.OnApply(this);
            _buffs.Add(buff);
            OnBuffApplied?.Invoke(buff);
            return;
        }

        //以下すでに同名のバフがある場合の処理
        switch (data.StackPolicy)
        {
            case StackPolicy.StackMagnitude:
                for (int i = 0; i < application.Stacks; i++) existing.AddStack(this,application.Duration);
                break;
            case StackPolicy.StackDuration:
                BuffModel additional = data.CreateModel(source, application.Stacks);
                additional.OnApply(this);
                _buffs.Add(additional);
                OnBuffApplied?.Invoke(additional);
                break;
            case StackPolicy.Refresh:
            default:
                existing.RefreshDuration(application.Duration);
                break;
        }
    }

    private void TickBuffs(float deltaTime)
    {
        for (int i = _buffs.Count - 1; i >= 0; i--)
        {
            _buffs[i].Tick(deltaTime, this);
            if (_buffs[i].Expired)
            {
                _buffs[i].OnRemove(this);
                OnBuffRemoved?.Invoke(_buffs[i]);
                _buffs.RemoveAt(i);
            }
        }
    }
}
