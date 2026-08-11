using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バフ・デバフの定義データ (ScriptableObject)
/// 効果時間・重ねがけポリシー・付与するステータス補正／継続ダメージを保持する
/// 「スピードブースト」「酸」のような、ステータス補正や継続ダメージだけで表現できるバフは、
/// このクラスのアセットとしてエディタ上で作成するだけでよい（C#クラスを分ける必要はない）
/// ステータス補正・継続ダメージ以外の独自効果を持たせたい場合は、BuffDataとBuffModelをそれぞれ継承する
/// （ImplantData/ImplantModel、ToolData/ToolModelと同じパターン）
/// </summary>
[CreateAssetMenu(menuName = "Entity/BuffData")]
public class BuffData : ScriptableObject
{
    [SerializeField, Header("重複・スタック判定に使う識別子")] private string _buffId;
    [SerializeField] private StackPolicy _stackPolicy;
    [SerializeField] private int _maxStacks = 1;

    [SerializeField, Header("ステータス補正（自身のStatValueへ加算する）")] private StatModifier[] _statModifiers;

    [SerializeField, Header("継続ダメージ（0なら発生しない）")] private float _tickInterval;
    [SerializeField] private float _tickDamage;
    [SerializeField] private DamageTarget _tickDamageTarget = DamageTarget.ArmorOnly;

    public string BuffId => _buffId;
    public StackPolicy StackPolicy => _stackPolicy;
    public int MaxStacks => _maxStacks;
    public IReadOnlyList<StatModifier> StatModifiers => _statModifiers;
    public float TickInterval => _tickInterval;
    public float TickDamage => _tickDamage;
    public DamageTarget TickDamageTarget => _tickDamageTarget;

    /// <summary>実行時インスタンスを生成する。独自の効果を持つバフはBuffModelを継承しoverrideする</summary>
    public virtual BuffModel CreateModel(EntityModel source,float duration, int initialStacks = 1)
    {
        return new BuffModel(this, source,duration, initialStacks);
    }
}

/// <summary>重ねがけ時の挙動</summary>
public enum StackPolicy
{
    /// <summary>再付与時は残り時間だけ延長する（スタックは増やさない）</summary>
    Refresh,
    /// <summary>再付与のたびに独立したタイマーを持つ別インスタンスとして積み増す</summary>
    StackDuration,
    /// <summary>同一インスタンスのスタック数を増やし、補正量・継続ダメージを倍加する（残り時間も延長）</summary>
    StackMagnitude,
}
