using System.Collections.Generic;

/// <summary>銃ステータスの種別。パッシブ効果が銃のステータスを補正する際のキーとして使う</summary>
public enum GunStatType { MagCap, ReloadTime, FireRate, SpreadPenalty_Move, SpreadPenalty_Fire, MaxSpreadPenalty_fire, SpreadLoss }

/// <summary>投射物ステータスの種別。パッシブ効果が弾のステータスを補正する際のキーとして使う</summary>
public enum PjtlStatType { BulletSpeed, BulletLifetime, Penetration, Damage }

/// <summary>
/// 装備中パッシブ効果による銃・投射物ステータスへの補正を集計するクラス（Pull型）
/// GunModel/Projectile側が使用の都度これを参照して基礎値に合成する。GunStats/PjtlStats自体は書き換えない。
/// これにより、武器の持ち替えや新規入手時にも常に最新の補正が反映される。
/// </summary>
public class PassiveModifierSet
{
    private readonly Dictionary<GunStatType, float> _gunFlat = new Dictionary<GunStatType, float>();
    private readonly Dictionary<GunStatType, float> _gunMultiplier = new Dictionary<GunStatType, float>();
    private readonly Dictionary<PjtlStatType, float> _pjtlFlat = new Dictionary<PjtlStatType, float>();
    private readonly Dictionary<PjtlStatType, float> _pjtlMultiplier = new Dictionary<PjtlStatType, float>();
    private readonly List<IProjectileHitModifier> _hitModifiers = new List<IProjectileHitModifier>();

    public void AddGunFlat(GunStatType type, float amount) => _gunFlat[type] = Get(_gunFlat, type) + amount;
    public void RemoveGunFlat(GunStatType type, float amount) => AddGunFlat(type, -amount);
    public void AddGunMultiplier(GunStatType type, float amount) => _gunMultiplier[type] = Get(_gunMultiplier, type) + amount;
    public void RemoveGunMultiplier(GunStatType type, float amount) => AddGunMultiplier(type, -amount);

    public void AddPjtlFlat(PjtlStatType type, float amount) => _pjtlFlat[type] = Get(_pjtlFlat, type) + amount;
    public void RemovePjtlFlat(PjtlStatType type, float amount) => AddPjtlFlat(type, -amount);
    public void AddPjtlMultiplier(PjtlStatType type, float amount) => _pjtlMultiplier[type] = Get(_pjtlMultiplier, type) + amount;
    public void RemovePjtlMultiplier(PjtlStatType type, float amount) => AddPjtlMultiplier(type, -amount);

    /// <summary>基礎値に登録中の実数・倍率補正を合成して返す（(base+flat)*(1+multiplier)）</summary>
    public float ApplyGun(GunStatType type, float baseValue) => (baseValue + Get(_gunFlat, type)) * (1f + Get(_gunMultiplier, type));

    /// <summary>基礎値に登録中の実数・倍率補正を合成して返す（(base+flat)*(1+multiplier)）</summary>
    public float ApplyPjtl(PjtlStatType type, float baseValue) => (baseValue + Get(_pjtlFlat, type)) * (1f + Get(_pjtlMultiplier, type));

    private static float Get<T>(Dictionary<T, float> dict, T key) => dict.TryGetValue(key, out float value) ? value : 0f;

    // -------------------------------------------------------
    // 命中時のEffectAction動的補正（IProjectileHitModifier）
    // スカラー値の加算では表現できない、命中相手・弾自身の状態に応じた補正はこちらへ登録する
    // -------------------------------------------------------

    public void AddHitModifier(IProjectileHitModifier modifier) => _hitModifiers.Add(modifier);
    public void RemoveHitModifier(IProjectileHitModifier modifier) => _hitModifiers.Remove(modifier);

    /// <summary>登録中のIProjectileHitModifierを順に適用し、補正後のEffectActionを返す</summary>
    public EffectAction ApplyHitModifiers(EffectAction baseAction, ProjectileHitContext context)
    {
        foreach (IProjectileHitModifier modifier in _hitModifiers) baseAction = modifier.Modify(baseAction, context);
        return baseAction;
    }
}
