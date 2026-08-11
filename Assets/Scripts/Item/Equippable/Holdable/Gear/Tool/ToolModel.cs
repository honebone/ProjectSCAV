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
        _data.StatModifiers.ApplyTo(owner);
    }

    /// <summary>放した瞬間：付与したパッシブ効果を取り除く</summary>
    public virtual void OnRemove(EntityModel owner)
    {
        _data.StatModifiers.RemoveFrom(owner);
    }
}
