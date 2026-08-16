using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestImplantModel : ImplantModel
{
    private IReadOnlyList<GunStatModifier> _gunStatModifiersOnStanding;
    private bool _isActive;
    public TestImplantModel(TestImplantData data) : base(data)
    {
        _gunStatModifiersOnStanding = data.GunStatModifiersOnStanding;
    }

    public override void OnTick(float deltaTime, EntityModel owner)
    {
        base.OnTick(deltaTime, owner);

        if (owner != null && owner is IMovable movable)
        {
            if (!movable.IsMoving && !_isActive)
            {
                _isActive = true;
                _gunStatModifiersOnStanding.ApplyTo(owner);
            }
            else if(movable.IsMoving && _isActive)
            {
                _isActive = false;
                _gunStatModifiersOnStanding.RemoveFrom(owner);
            }
        }
    }

    public override void OnRemove(EntityModel owner)
    {
        base.OnRemove(owner);

        if (_isActive)
        {
            _isActive = false;
            _gunStatModifiersOnStanding.RemoveFrom(owner);
        }
    }
}
