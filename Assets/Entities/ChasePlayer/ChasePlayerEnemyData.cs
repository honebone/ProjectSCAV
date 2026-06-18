using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/ChasePlayerEnemyData")]
public class ChasePlayerEnemyData : EntityStatsData
{
    public override EntityModel CreateModel(EntityView view)
    {
        return new ChasePlayerEnemyModel(this, view, view, view,view);
    }
}
