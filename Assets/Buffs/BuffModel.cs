using UnityEngine;

/// <summary>
/// バフ・デバフの実行時インスタンス / IPassive
/// 効果時間・スタック数の管理はこのクラスで完結させる（サブクラスでは触らない）
/// 実際の効果（OnApply/OnRemove/OnTick）は既定でBuffDataのステータス補正・継続ダメージ設定を適用するのみで、
/// それ以外の独自効果を持たせたい場合はこのクラスを継承してoverrideする
/// </summary>
public class BuffModel : IPassive
{
    private protected readonly BuffData _data;
    private protected readonly EntityModel _source;
    private protected float _remainingTime;
    private protected int _stacks;

    public BuffData Data => _data;
    public string BuffId => _data.BuffId;
    /// <summary>このバフを付与した発生源。継続ダメージの発生源として使う</summary>
    public EntityModel Source => _source;
    public int Stacks => _stacks;
    public bool Expired => _remainingTime <= 0f;

    public BuffModel(BuffData data, EntityModel source,float duration, int initialStacks = 1)
    {
        _data = data;
        _source = source;
        _remainingTime = duration;
        _stacks = Mathf.Clamp(initialStacks, 1, Mathf.Max(1, data.MaxStacks));
    }

    /// <summary>バフが付与された瞬間の効果。既定ではDataのステータス補正を適用する</summary>
    public virtual void OnApply(EntityModel owner)
    {
        _data.StatModifiers.ApplyTo(owner, _stacks);
    }

    /// <summary>バフが除去された瞬間の効果。既定ではDataのステータス補正を除去する</summary>
    public virtual void OnRemove(EntityModel owner)
    {
        _data.StatModifiers.RemoveFrom(owner, _stacks);
    }

    /// <summary>残り時間を現在値と指定した時間から長いほうにリセットする（StackPolicy.Refresh等で使用）</summary>
    public void RefreshDuration(float newDuration)
    {
        _remainingTime = Mathf.Max(_remainingTime, newDuration);
    }

    /// <summary>スタック数を1増やし、補正を再適用してから残り時間を延長する（StackPolicy.StackMagnitude用）</summary>
    public void AddStack(EntityModel owner,float duration)
    {
        if (_stacks >= _data.MaxStacks)
        {
            RefreshDuration(duration);
            return;
        }

        OnRemove(owner);
        _stacks++;
        OnApply(owner);
        RefreshDuration(duration);
    }

    /// <summary>毎フレーム呼ぶ。残り時間の減算と、効果時間中の処理（継続ダメージなど）を行う</summary>
    public void Tick(float deltaTime, EntityModel owner)
    {
        _remainingTime -= deltaTime;
        OnTick(deltaTime, owner);
    }

    /// <summary>効果時間中、毎フレーム発生する処理。既定ではDataのTickInterval/TickDamage設定に従い継続ダメージを発生させる</summary>
    protected virtual void OnTick(float deltaTime, EntityModel owner)
    {
        
    }
}
