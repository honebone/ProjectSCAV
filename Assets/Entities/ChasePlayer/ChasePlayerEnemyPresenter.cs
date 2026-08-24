using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerEnemyPresenter : EntityPresenter
{
    public override void Bind()
    {
        base.Bind();

        if (_model is ILoadoutable loadoutable)
        {
            if (_view is IItemVisualizer visualizer)
            {
                loadoutable.Loadout.OnActiveItemChanged += visualizer.OnItemHeld;
            }
            else DevLog.Error("[ChasePlayerEnemyPresenter] View‚ªIItemVisualizer‚ðŽÀ‘•‚µ‚Ä‚¢‚Ü‚¹‚ñ");
        }
        if (_model is ChasePlayerEnemyModel model && _view is ChasePlayerEnemyView view) model.OnStateChanged += view.OnStateChanged;
    }
}
