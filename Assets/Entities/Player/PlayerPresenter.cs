using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerPresenter : EntityPresenter
{
    public override void Bind()
    {
        base.Bind();

        if (_model is PlayerModel model)
        {
            if (_view is PlayerView view)
            {
                model.Loadout.OnActiveItemChanged += view.OnItemHeld;
                model.OnJetpackStart += view.OnJetpackStart;
                model.OnJetpackEnd += view.OnJetpackEnd;
            }
            else DevLog.Error("[PlayerPresenter] View‚ªPlayerView‚Å‚Í‚ ‚è‚Ü‚¹‚ñ");

            
        }
    }
}
