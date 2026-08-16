/// <summary>
/// 投射物命中時のEffectActionを検査・補正するパッシブ効果のインターフェース
/// 「飛翔距離に応じてダメージ増加」「HPが一定割合以下の敵に追加デバフ」のような、
/// 命中の瞬間にしか分からない値（ProjectileHitContext）を使う効果はこれを実装する
///
/// LoadoutModel.PassiveModifiers（PassiveModifierSet）へOnApply/OnRemoveで自己登録する
/// （BuffModel : IPassiveと同様、実装クラス自身がインターフェースを実装する形を想定）
/// </summary>
public interface IProjectileHitModifier
{
    /// <summary>
    /// 命中時のEffectActionを検査し、必要なら補正した新しいEffectActionを返す
    /// 補正が不要な場合はbaseActionをそのまま返す
    /// </summary>
    EffectAction Modify(EffectAction baseAction, ProjectileHitContext context);
}
