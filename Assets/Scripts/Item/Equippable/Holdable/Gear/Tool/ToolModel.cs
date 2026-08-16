using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具のModel / IUsable, IPassive
/// 持つだけでパッシブ効果（IPassive）またはUse()でアクティブ効果（IUsable）を持つ
/// 既定ではDataのステータス補正を自動適用するのみ。それ以外の独自効果を持たせたい場合はこのクラスを継承しoverrideする
/// </summary>
public class ToolModel : GearModel, IPassive
{
    private readonly ToolData _data;

    /// <summary>持っている間だけ非null。OnTickが「実際に持っている間」だけ動くようにするためのガードに使う</summary>
    private EntityModel _owner;

    public ToolModel(ToolData data) : base(data)
    {
        _data = data;
    }

    // -------------------------------------------------------
    // IPassive
    // -------------------------------------------------------

    public override void OnHold(EntityModel owner)
    {
        base.OnHold(owner);
        // 持ったときにパッシブ効果を付与する
        OnApply(owner);
    }

    public override void OnUnhold(EntityModel owner)
    {
        base.OnUnhold(owner);
        // 放したときにパッシブ効果を取り除く
        OnRemove(owner);
    }

    /// <summary>持った瞬間：EntityModelへパッシブ効果を付与する</summary>
    public virtual void OnApply(EntityModel owner)
    {
        _data.Modifiers.ApplyTo(owner);
        _owner = owner;
    }

    /// <summary>放した瞬間：付与したパッシブ効果を取り除く</summary>
    public virtual void OnRemove(EntityModel owner)
    {
        _data.Modifiers.RemoveFrom(owner);
        _owner = null;
    }

    // -------------------------------------------------------
    // 常時Tickでの条件判定（実際に持っている間のみ動く）
    // -------------------------------------------------------

    /// <summary>
    /// HoldableSlot.Tickはスロットに入っている間（持ち替えて手放していても）毎フレーム呼ばれるため、
    /// 実際に持っている間（OnApply〜OnRemoveの間）だけOnTickが動くようここでガードする
    /// </summary>
    public override void Tick(float deltaTime, EntityModel user)
    {
        base.Tick(deltaTime, user);
        if (_owner == null) return;
        OnTick(deltaTime, _owner);
    }

    /// <summary>
    /// 実際に持っている間、毎フレーム呼ばれる（既定では何もしない）
    /// 「マガジン最後の1発はダメージ増加」のような、常時条件判定が必要なパッシブはこれをoverrideする
    /// </summary>
    protected virtual void OnTick(float deltaTime, EntityModel owner) { }
}
