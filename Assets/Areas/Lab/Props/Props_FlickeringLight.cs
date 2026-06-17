using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Props_FlickeringLight : Props_Light
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Light2D _light;
    [SerializeField] private ParticleSystem _sparks;

    [Header("Settings")]
    [Range(0, 1)][SerializeField] private List<float> _dutyCycles;
    [SerializeField] private Vector2 FlickerSpeed = new Vector2(0.02f, 0.2f);
    [SerializeField] private float _sparkChance = 50;

    [Header("Intensity")]
    [SerializeField] private float _baseIntensity = 1.0f;
    [SerializeField] private float _fadeDuration = 0.05f; // 残光の速さ

    private Material _targetMaterial;
    private float _dutyCycle = 0.5f;
    private bool _isEmitting = false;
    private bool _isAvailable = true;
    private Tween _intensityTween;

    void Start()
    {
        _targetMaterial = _spriteRenderer.material;
        _baseIntensity=_light.intensity;
        _dutyCycle = _dutyCycles.Choice();

        if(_dutyCycle == 0)
        {
            _isAvailable = false;
        }
        else
        {
            // 最初のコルーチンを開始
            StartCoroutine(FlickerRoutine());
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (_isAvailable)
        {
            // デューティー比に基づきON/OFFを決定
            bool nextState = Random.value < _dutyCycle;

            if (nextState != _isEmitting)
            {
                UpdateLightState(nextState);
            }

            // 次の判定までの待機時間をランダムに設定（壊れかけの不規則さ）
            yield return new WaitForSeconds(FlickerSpeed.Range());
        }
    }

    private void UpdateLightState(bool on)
    {
        _isEmitting = on;

        // 既存のTweenがあれば止める
        _intensityTween?.Kill();

        if (on)
        {
            // --- 点灯時 ---
            _targetMaterial.SetInt("_Emit", 1);
            _light.enabled = true;

            // DOTweenで瞬時に（またはわずかに遊びを持って）明るくする
            _intensityTween = DOTween.To(() => 0f, x => SetIntensity(x), _baseIntensity, 0.02f)
                .SetEase(Ease.OutExpo);

            // 調子が悪い（Duty比が低い）ほど、点灯の瞬間に火花が散る
            float sparkThreshold = 1.0f - _dutyCycle;
            if ((sparkThreshold * _sparkChance).Dice())
            {
                _sparks.Emit(Random.Range(1, 5));
            }
        }
        else
        {
            // --- 消灯時 ---
            _targetMaterial.SetInt("_Emit", 0);

            // DOTweenで残光を表現（ジワッと消える）
            _intensityTween = DOTween.To(() => _baseIntensity, x => SetIntensity(x), 0f, _fadeDuration)
                .OnComplete(() => _light.enabled = false);
        }
    }

    protected override void EnterEmergencyMode()
    {
        if (_isAvailable)
        {
            _intensityTween?.Kill();

        }
    }

    protected override void SetIntensity(float value)
    {
        //わずかなランダムノイズを加えて「電圧の不安定さ」を出す
            float jitter = _isEmitting ? Random.Range(0.9f, 1.1f) : 1.0f;
        float finalValue = value * jitter;

        _targetMaterial.SetFloat("_EmitIntensity", finalValue);
        _light.intensity = finalValue;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // オブジェクト破棄時にTweenをクリーンアップ
        _intensityTween?.Kill();
    }
}
