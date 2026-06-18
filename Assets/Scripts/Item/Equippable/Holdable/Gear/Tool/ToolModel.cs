using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具のModel / IUsable, IPassive
/// 持つだけでパッシブ効果（IPassive）またはUse()でアクティブ効果（IUsable）を持つ
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

    public override void OnHold()
    {
        base.OnHold();
        // 持ったときにパッシブ効果を付与する
        // 所持者はOnHold/OnUnholdのタイミングでOnEquip/OnUnequipを呼ぶ
    }

    public override void OnUnhold()
    {
        base.OnUnhold();
    }

    /// <summary>装備時：EntityModelへパッシブ効果を付与する</summary>
    public void OnEquip(EntityModel owner)
    {
        // サブクラスでステータス補正などを実装する
    }

    /// <summary>取り外し時：付与したパッシブ効果を除去する</summary>
    public void OnUnequip(EntityModel owner)
    {
        // サブクラスでステータス補正の除去を実装する
    }
}
