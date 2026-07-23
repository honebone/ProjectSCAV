using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 銃のModel / IUsable
/// </summary>
public class GunModel : HoldableItemModel
{
    private readonly GunData _gunData;
    public GunStats GunStats { get; }

    private int _currentAmmo;
    private float _spreadPenalty_fire;
    private float _spreadPenalty_move;
    private float _fireIntervalTimer;//射撃間のタイマー
    private float _reloadTimeTemp;
    private float _reloadTimer;
    private bool _isReloading => _reloadTimer > 0;
    private int _burstCount;
    private bool _isBursting => _burstCount > 0;
    private bool _isPullingTrigger;
    public int CurrentAmmo => _currentAmmo;

    public event Action<float> OnSpreadChanged;
    public event Action<FireParams> OnFired;
    public event Action<float> OnReloadStart;
    public event Action<float, float> OnReloading;
    public event Action<float> OnReloadCanceled;
    public event Action<float> OnReloadCompleted;

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

        if (user is IMovable movable)
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

        if (_fireIntervalTimer == 0)
        {
            if (_gunData.FireType == FireType.auto && _isPullingTrigger) TryFire(user);
            if (_isBursting) TryFire(user);
        }

        if(_reloadTimer > 0)
        {
            _reloadTimer -= deltaTime;
            OnReloading?.Invoke(_reloadTimer, _reloadTimeTemp);
            if (_reloadTimer < 0)
            {
                _reloadTimer = 0;
            }
        }

        OnSpreadChanged?.Invoke(GunStats.PjtlStats.Spread.Value + _spreadPenalty_fire + _spreadPenalty_move);
    }

    public override void OnUnhold()
    {
        base.OnUnhold();
    }

    // -------------------------------------------------------
    // IUsable
    // -------------------------------------------------------

    /// <summary>射撃</summary>
    public override void Use(EntityModel user)
    {
        if (!_isBursting && _gunData.FireType == FireType.burst) _burstCount = _gunData.burstRounds;
        TryFire(user);
        _isPullingTrigger = true;
    }

    public override void StopUsing(EntityModel user)
    {
        _isPullingTrigger = false;
    }

    public override void Reload(EntityModel user)
    {
        if (!_isPullingTrigger && !_isBursting && _currentAmmo < GunStats.MagCap.IntValue)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        _reloadTimeTemp = GunStats.ReloadTime.Value;
        _reloadTimer = _reloadTimeTemp;
        DevLog.Log($"reload start:{_reloadTimeTemp}s");
    }

    private void CancelReload()
    {
        DevLog.Log("reload canceled");
        if (_isReloading)
        {
            OnReloadCanceled?.Invoke(_reloadTimeTemp);
            _reloadTimer = 0;
            _reloadTimeTemp = 0;
        }
    }

    /// <summary>
    /// マガジンを補充
    /// </summary>
    /// <param name="amount">リロードする弾数</param>
    /// <param name="exceedMagCap">マガジン容量を超えてリロードするか</param>
    /// <returns>実際にリロードした数</returns>
    private int FeedAmmo(int amount, bool exceedMagCap = false)
    {
        // 追加する弾数が0以下の場合は処理しない
        if (amount <= 0) return 0;

        int initialAmmo = _currentAmmo;

        if (exceedMagCap)
        {
            _currentAmmo += amount;
        }
        else
        {
            // 現在の弾数がすでにマガジン容量を超えている場合は、現在の弾数を上限として扱う（弾が減るのを防ぐため）
            int maxCap = Mathf.Max(GunStats.MagCap.IntValue, _currentAmmo);

            // 弾を追加し、上限を超えないようにクランプする
            _currentAmmo = Mathf.Min(_currentAmmo + amount, maxCap);
        }

        // 実際にリロードされた弾数を返す
        return _currentAmmo - initialAmmo;
    }

    public void TryFire(EntityModel user)
    {
        if (_currentAmmo <= 0) return;
        if (_fireIntervalTimer > 0) return;

        if (user is ILookable lookable)
        {
            if(_isReloading)CancelReload();

            PjtlSnapshot snapshot = PjtlSnapshot.From(GunStats.PjtlStats);
            snapshot.Spread += _spreadPenalty_fire + _spreadPenalty_move;

            List<Faction> targetFactions = new List<Faction>(user.Hostiles);
            targetFactions.Add(Faction.obstacle);

            FireParams fireParams = new FireParams(targetFactions, user.Position, lookable.LookAt, snapshot);
            OnFired?.Invoke(fireParams);
            _currentAmmo--;

            _spreadPenalty_fire += GunStats.SpreadPenalty_Fire.Value;
            if (_spreadPenalty_fire > GunStats.MaxSpreadPenalty_fire.Value) _spreadPenalty_fire = GunStats.MaxSpreadPenalty_fire.Value;

            if (_isBursting) _burstCount--;
            float interval = _isBursting ? 1f / _gunData.BurstRate : 1f / GunStats.FireRate.Value;
            SetFireIntervalTimer(interval);
        }
        else DevLog.Error("userがILookableではありません");
    }

    public void SetFireIntervalTimer(float interval)
    {
        _fireIntervalTimer = interval;
    }
}