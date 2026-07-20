using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class GunVisual : IItemVisual
{
    [SerializeField] private Transform _muzzle;           // 銃口の位置
    [SerializeField] private Light2D _muzzleFlash;
    [SerializeField] private List<Transform> _spreadLines;
    [SerializeField] private ParticleSystem _cartridgeParticle;

    private GunModel _model;
    private Tween _intensityTween;
    private float _baseIntensity;

    public override void Subscribe(HoldableItemModel model)
    {
        _model = model as GunModel;
        if (_model == null) return;

        _baseIntensity = _muzzleFlash.intensity;

        _model.OnSpreadChanged += OnSpreadChanged;
        _model.OnFired += HandleFired;
        _model.OnReloaded += HandleReloaded;
    }

    public override void Unsubscribe()
    {
        if (_model == null) return;

        _model.OnSpreadChanged -= OnSpreadChanged;
        _model.OnFired -= HandleFired;
        _model.OnReloaded -= HandleReloaded;
    }

    private void HandleFired(FireParams fireParams)
    {
        // マズルフラッシュ
        _intensityTween?.Kill();
        _muzzleFlash.enabled = true;

        _intensityTween = DOTween.To(() => _baseIntensity, x => SetIntensity(x), 0f, 0.25f).OnComplete(() => _muzzleFlash.enabled = false);


        // 薬莢
        _cartridgeParticle?.Emit(1);

        //FireParams fire = fireParams;
        //fire.FirePos = _muzzle.position;
        fireParams.SetFirePos(_muzzle.position);
        _projectileSpawner.SpawnProjectile(fireParams);
    }

    private void OnSpreadChanged(float spread)
    {
        _spreadLines[0].localRotation = Quaternion.Euler(0f, 0f, 45 + (spread / 2f) - 90);
        _spreadLines[1].localRotation = Quaternion.Euler(0f, 0f, 45 - (spread / 2f) - 90);
    }

    private void HandleReloaded() { /* リロードアニメ再生など */ }

    private void SetIntensity(float value)
    {
        _muzzleFlash.intensity = value;
    }
}
