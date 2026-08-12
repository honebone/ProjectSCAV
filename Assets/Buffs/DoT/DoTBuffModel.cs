using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTBuffModel : BuffModel
{
    private float _tickTimer;

    private float _tickInterval;
    private float _tickDamage;
    private DamageTarget _tickDamageTarget;

    public DoTBuffModel(
        BuffData data,
        EntityModel source,
        float duration,
        float tickInterval,
        float tickDamage,
        DamageTarget tickDamageTarget,
        int initialStacks = 1) : base(data, source, duration, initialStacks)
    {
        _tickInterval = tickInterval;
        _tickDamage = tickDamage;
        _tickDamageTarget = tickDamageTarget;
    }

    protected override void OnTick(float deltaTime, EntityModel owner)
    {
        base.OnTick(deltaTime, owner);

        if (_tickInterval <= 0f) return;

        _tickTimer += deltaTime;
        if (_tickTimer < _tickInterval) return;

        _tickTimer -= _tickInterval;
        owner.Resolve(new EffectAction(_source, damageAmount: _tickDamage * _stacks, damageTarget: _tickDamageTarget));
    }
}
