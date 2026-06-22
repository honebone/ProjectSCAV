using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// èeÇÃModel / IUsable
/// </summary>
public class GunModel : HoldableItemModel
{
    private readonly GunData _gunData;
    public GunStats GunStats { get; }

    private int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;

    public event Action<FireParams> OnFired;
    public event Action OnReloaded;

    public GunModel(GunData data) : base(data)
    {
        _gunData = data;
        GunStats = new GunStats(_gunData);

        _currentAmmo = GunStats.MagCap.IntValue;//test
    }

    // -------------------------------------------------------
    // IUsable
    // -------------------------------------------------------

    /// <summary>éÀåÇ</summary>
    public override void Use(EntityModel user)
    {
        if (_currentAmmo <= 0) return;
        //_currentAmmo--;

        PjtlSnapshot snapshot = PjtlSnapshot.From(GunStats.PjtlStats);

        List<Faction> targetFactions = new List<Faction>(user.Hostiles);
        targetFactions.Add(Faction.obstacle);
        if (user is ILookable lookable)
        {
            FireParams fireParams = new FireParams(targetFactions, user.Position, lookable.LookAt, snapshot);
            TryFire(fireParams);
        }
        else DevLog.Error("userÇ™ILookableÇ≈ÇÕÇ†ÇËÇ‹ÇπÇÒ");
    }

    public override void StopUsing(EntityModel user) { }

    public void TryFire(FireParams fireParams)
    {
        OnFired?.Invoke(fireParams);
    }
}