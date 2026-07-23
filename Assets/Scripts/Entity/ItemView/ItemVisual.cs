using UnityEngine;

public abstract class IItemVisual : MonoBehaviour
{
    private protected EntityView _entityView;
    public void Init(EntityView entityView)
    {
        _entityView = entityView;
    }

    public abstract void Subscribe(HoldableItemModel model);
    public abstract void Unsubscribe();
}