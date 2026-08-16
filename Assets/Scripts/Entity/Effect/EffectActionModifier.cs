using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EffectActionへの補正1件分
/// EffectActionは不変（コンストラクタでしか値を作れない）なため、IProjectileHitModifier.Modify()等で
/// 一部の値だけを変えた新しいEffectActionを作りたい場面で、コンストラクタの全引数をコピーし直す手間を減らすために使う
/// </summary>
[System.Serializable]
public struct EffectActionModifier
{
    [SerializeField] private float _damageFlat;
    [SerializeField] private float _damageMultiplier;   // +0.2なら20%増（PassiveModifierSet等と同じ規約）
    [SerializeField] private float _healFlat;
    [SerializeField] private float _healMultiplier;
    [SerializeField] private BuffApplication[] _additionalBuffs;

    public float DamageFlat => _damageFlat;
    public float DamageMultiplier => _damageMultiplier;
    public float HealFlat => _healFlat;
    public float HealMultiplier => _healMultiplier;
    public IReadOnlyList<BuffApplication> AdditionalBuffs => _additionalBuffs;

    public EffectActionModifier(float damageFlat = 0f, float damageMultiplier = 0f,
        float healFlat = 0f, float healMultiplier = 0f, IReadOnlyList<BuffApplication> additionalBuffs = null)
    {
        _damageFlat = damageFlat;
        _damageMultiplier = damageMultiplier;
        _healFlat = healFlat;
        _healMultiplier = healMultiplier;
        _additionalBuffs = additionalBuffs == null ? null : new List<BuffApplication>(additionalBuffs).ToArray();
    }

    /// <summary>baseActionへこの補正を適用した新しいEffectActionを返す（(base+flat)*(1+multiplier)）</summary>
    public EffectAction Apply(EffectAction baseAction)
    {
        float damage = (baseAction.DamageAmount + _damageFlat) * (1f + _damageMultiplier);
        float heal = (baseAction.HealAmount + _healFlat) * (1f + _healMultiplier);

        List<BuffApplication> buffs = new List<BuffApplication>(baseAction.Buffs);
        if (_additionalBuffs != null) buffs.AddRange(_additionalBuffs);

        return new EffectAction(baseAction.Source, damage, baseAction.DamageTarget, heal, baseAction.HealTarget, buffs);
    }
}
