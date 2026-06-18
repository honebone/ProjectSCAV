using UnityEngine;

public abstract class IItemVisual : MonoBehaviour
{
    private protected IProjectileSpawner _projectileSpawner;
    public void Init(IProjectileSpawner projectileSpawner)
    {
        _projectileSpawner = projectileSpawner;
    }

    public abstract void Subscribe(HoldableItemModel model);
    public abstract void Unsubscribe();
}