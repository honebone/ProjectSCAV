using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuffData/DoT")]
public class DoTBuffData : BuffData
{
    [SerializeField] private float _tickInterval;
    [SerializeField] private float _tickDamage;
    [SerializeField] private DamageTarget _tickDamageTarget = DamageTarget.ArmorOnly;

    public float TickInterval => _tickInterval;
    public float TickDamage => _tickDamage;
    public DamageTarget TickDamageTarget => _tickDamageTarget;

    public override BuffModel CreateModel(EntityModel source, float duration, int initialStacks = 1)
    {
        return new DoTBuffModel(this, source, duration, initialStacks);
    }
}
