using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手に持てるアイテムの基底Model
/// 持ち替え時間・持ったとき/しまったときのコールバックを持つ
/// </summary>
public abstract class HoldableItemModel : EquippableItemModel,IUsable
{
    private readonly HoldableItemData _holdableData;

    /// <summary>持ち替えにかかる時間（秒）</summary>
    public float DrawTime => _holdableData.DrawTime;
    public GameObject Prefab => _holdableData.VisualPrefab;
    public float UseTime => _holdableData.UseTime;

    protected HoldableItemModel(HoldableItemData data) : base(data)
    {
        _holdableData = data;
    }

    /// <summary>手に持ったとき</summary>
    public virtual void OnHold() {  }

    /// <summary>しまったとき</summary>
    public virtual void OnUnhold() { }

    /// <summary>効果を発動して消費する</summary>
    public virtual void Use(EntityModel user)
    {
        // サブクラスで効果を実装する
        // 消費処理はInventoryModel側から行う
    }

    public virtual void StopUsing(EntityModel user) { }
}
