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
    private float _spreadPenalty_fire;
    private float _spreadPenalty_move;
    private float _fireIntervalTimer;
    private bool _isPullingTrigger;
    public int CurrentAmmo => _currentAmmo;

    public event Action<float> OnSpreadChanged;
    public event Action<FireParams> OnFired;
    public event Action OnReloaded;

    public GunModel(GunData data) : base(data)
    {
        _gunData = data;
        GunStats = new GunStats(_gunData);

        _currentAmmo = GunStats.MagCap.IntValue;//test
    }

    public override void Tick(float deltaTime, EntityModel user)
    {
        if (_spreadPenalty_fire > 0)
        {
            _spreadPenalty_fire -= deltaTime * GunStats.SpreadLoss.Value;
            if (_spreadPenalty_fire < 0) _spreadPenalty_fire = 0;
        }

        if(user is IMovable movable)
        {
            _spreadPenalty_move = movable.IsMoving ? GunStats.SpreadPenalty_Move.Value : 0;
        }

        if (_fireIntervalTimer > 0)
        {
            _fireIntervalTimer -= deltaTime;
            if (_fireIntervalTimer < 0)
            {
                _fireIntervalTimer = 0;
            }
        }

        if (_isPullingTrigger && _fireIntervalTimer == 0)
        {
            if (_gunData.FireType == FireType.auto) TryFire(user);
        }

        OnSpreadChanged?.Invoke(GunStats.PjtlStats.Spread.Value + _spreadPenalty_fire + _spreadPenalty_move);
    }

    // -------------------------------------------------------
    // IUsable
    // -------------------------------------------------------

    /// <summary>éÀåÇ</summary>
    public override void Use(EntityModel user)
    {
        TryFire(user);
        _isPullingTrigger = true;
    }

    public override void StopUsing(EntityModel user)
    {
        _isPullingTrigger = false;
    }

    public void TryFire(EntityModel user)
    {
        if (_currentAmmo <= 0) return;
        if (_fireIntervalTimer > 0) return;
        //_currentAmmo--;

        if (user is ILookable lookable)
        {
            PjtlSnapshot snapshot = PjtlSnapshot.From(GunStats.PjtlStats);
            snapshot.Spread += _spreadPenalty_fire + _spreadPenalty_move;

            List<Faction> targetFactions = new List<Faction>(user.Hostiles);
            targetFactions.Add(Faction.obstacle);

            FireParams fireParams = new FireParams(targetFactions, user.Position, lookable.LookAt, snapshot);
            OnFired?.Invoke(fireParams);
            _spreadPenalty_fire += GunStats.SpreadPenalty_Fire.Value;
            if(_spreadPenalty_fire > GunStats.MaxSpreadPenalty_fire.Value) _spreadPenalty_fire = GunStats.MaxSpreadPenalty_fire.Value;
            SetFireIntervalTimer();
        }
        else DevLog.Error("userÇ™ILookableÇ≈ÇÕÇ†ÇËÇ‹ÇπÇÒ");
    }

    public void SetFireIntervalTimer() { 
        _fireIntervalTimer = 1f / GunStats.FireRate.Value;
    }
}