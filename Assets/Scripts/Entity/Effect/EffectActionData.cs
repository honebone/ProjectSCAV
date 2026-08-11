using UnityEngine;

/// <summary>
/// EffectActionをインスペクター上で作成するためのテンプレート
/// EffectAction.Source（実行時にしか決まらない発生源）を除いた内容をシリアライズし、
/// Create()で発生源を渡して実際のEffectActionを組み立てる
/// 「リロード完了時に発火する効果」のような、トリガーの中身をデータ側で自由に設定したい場合に使う
/// </summary>
[System.Serializable]
public class EffectActionData
{
    [SerializeField] private float _damageAmount;
    [SerializeField] private DamageTarget _damageTarget = DamageTarget.HPAndArmor;
    [SerializeField] private float _healAmount;
    [SerializeField] private HealTarget _healTarget = HealTarget.HPAndArmor;
    [SerializeField] private BuffApplication[] _buffs;

    public EffectAction Create(EntityModel source)
    {
        return new EffectAction(source, _damageAmount, _damageTarget, _healAmount, _healTarget, _buffs);
    }
}
