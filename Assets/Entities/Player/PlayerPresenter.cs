using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPresenter : EntityPresenter
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
            else DevLog.Error("[PlayerPresenter] View‚ªIItemVisualizer‚ðŽÀ‘•‚µ‚Ä‚¢‚Ü‚¹‚ñ");
        }
    }
}
