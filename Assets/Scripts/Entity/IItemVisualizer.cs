using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Viewが実装する：アイテムオブジェクトの生成・更新ができることを示す
public interface IItemVisualizer
{
    HoldingItemView HoldingItemView { get; }
    void OnItemHeld(HoldableItemModel model);
    void UpdateAim(Vector2 lookAt);
}
