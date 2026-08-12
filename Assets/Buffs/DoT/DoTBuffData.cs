using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuffData/DoT")]
public class DoTBuffData : BuffData
{
    [SerializeField] private float _tickInterval;
    [SerializeField] private float _tickDamage;
    [SerializeField] private DamageTarget _tickDamageTarget = DamageTarget.ArmorOnly;

    public override BuffModel CreateModel(EntityModel source, float duration, int initialStacks = 1)
    {
        return new DoTBuffModel(this, source, duration, _tickInterval, _tickDamage, _tickDamageTarget, initialStacks);
    }
}
