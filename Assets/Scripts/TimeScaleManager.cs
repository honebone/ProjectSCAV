using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイムスケール(Time.timeScale)の変更を一元管理する静的クラス。
/// 演出（ヒットストップ・スローモーション等）によるタイムスケール変更は
/// 必ずこのクラス経由で行い、Time.timeScaleへの直接代入は行わないこと。
///
/// 複数箇所から同時に変更要求が来た場合は単純上書き（後から呼んだ方が勝つ）。
/// </summary>
public static class TimeScaleManager
{
    private static Tween _recoverTween;

    /// <summary>
    /// ゲーム起動時にシーンロードへ購読する。
    /// シーンをまたいでタイムスケールが1に戻らないまま持ち越される事故を防ぐための保険。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnRuntimeInitialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetScale();
    }

    /// <summary>
    /// タイムスケールを即座に指定値へ変更する。
    /// 進行中の「徐々に戻る」処理があれば打ち切る。
    /// </summary>
    public static void SetScale(float scale)
    {
        _recoverTween?.Kill();
        Time.timeScale = scale;
    }

    /// <summary>
    /// タイムスケールを即座にscaleへ変更し、delay秒後からduration秒かけて1へ戻す。
    /// 戻り処理はunscaledTime基準で進行するため、scaleが0（完全停止）でも正常に進む。
    /// </summary>
    public static void SetScaleThenRecover(float scale, float delay, float duration, Ease ease = Ease.Linear)
    {
        _recoverTween?.Kill();
        Time.timeScale = scale;

        _recoverTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, duration)
            .SetDelay(delay)
            .SetEase(ease)
            .SetUpdate(true) // unscaledTime基準で進行させる
            .OnKill(() => _recoverTween = null);
    }

    /// <summary>
    /// タイムスケールを即座に1へ戻す（進行中の戻り処理があれば打ち切る）。
    /// </summary>
    public static void ResetScale()
    {
        SetScale(1f);
    }
}
