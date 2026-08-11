using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンティティに対して働く作用（ダメージ・回復・バフ付与）を表すクラス
/// 継承階層は持たず、各効果の要素をパラメータとして保持する
/// 初期値（0や空リスト）でない要素だけがEntityModel.Resolve()で解決されるため、
/// 1つのインスタンスで「ダメージ＋デバフ付与」「複数バフの同時付与」のような複合効果も表現できる
/// </summary>
public class EffectAction
{
    private readonly EntityModel _source;
    private readonly float _damageAmount;
    private readonly DamageTarget _damageTarget;
    private readonly float _healAmount;
    private readonly HealTarget _healTarget;
    private readonly IReadOnlyList<BuffApplication> _buffs;

    /// <summary>発生源。弾丸を発射したエンティティなど。環境要因等はnullを許容する</summary>
    public EntityModel Source => _source;
    /// <summary>0ならダメージなし</summary>
    public float DamageAmount => _damageAmount;
    public DamageTarget DamageTarget => _damageTarget;
    /// <summary>0なら回復なし</summary>
    public float HealAmount => _healAmount;
    public HealTarget HealTarget => _healTarget;
    /// <summary>空なら付与するバフなし</summary>
    public IReadOnlyList<BuffApplication> Buffs => _buffs;

    public EffectAction(
        EntityModel source,
        float damageAmount = 0f, DamageTarget damageTarget = DamageTarget.HPAndArmor,
        float healAmount = 0f, HealTarget healTarget = HealTarget.HPAndArmor,
        IReadOnlyList<BuffApplication> buffs = null)
    {
        _source = source;
        _damageAmount = damageAmount;
        _damageTarget = damageTarget;
        _healAmount = healAmount;
        _healTarget = healTarget;
        _buffs = buffs ?? Array.Empty<BuffApplication>();
    }
}

/// <summary>ダメージを適用する対象（HP/Armorのどちらに作用するか）</summary>
public enum DamageTarget { HPAndArmor, HPOnly, ArmorOnly }

/// <summary>回復を適用する対象（HP/Armorのどちらに作用するか）</summary>
public enum HealTarget { HPAndArmor, HPOnly, ArmorOnly }

/// <summary>
/// EffectActionが付与するバフ1件分（対象バフ定義＋時間+スタック数）
/// インスペクター上でも設定できるようSerializeFieldで構成する
/// </summary>
[System.Serializable]
public struct BuffApplication
{
    [SerializeField] private BuffData _buff;
    [SerializeField] private float _duration;
    [SerializeField] private int _stacks;

    public BuffData Buff => _buff;
    public float Duration => _duration;
    public int Stacks => _stacks;

    public BuffApplication(BuffData buff,float duration, int stacks = 1)
    {
        _buff = buff;
        _duration = duration;
        _stacks = stacks;
    }
}
