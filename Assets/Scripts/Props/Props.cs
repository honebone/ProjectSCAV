using UnityEngine;

/// <summary>
/// マップ上に配置される装飾物の基底クラス
/// </summary>
public abstract class Props : MonoBehaviour
{
    // -------------------------------------------------------
    // 初期化
    // -------------------------------------------------------

    /// <summary>エリア生成後にエリアマネージャーから呼ばれる</summary>
    public virtual void Init(/* TODO: AreaManager manager */)
    {
        // TODO: manager.OnExitNormalMode += ExitNormalMode;
        // TODO: manager.OnEmergencyMode += EnterEmergencyMode;
        // TODO: manager.OnXxx += Xxx;
    }

    public void Test()
    {
        ExitNormalMode();
    }
    public void Test2()
    {
        EnterEmergencyMode();

    }

    // -------------------------------------------------------
    // イベント
    // -------------------------------------------------------

    /// <summary>
    /// 故障時に呼ばれる（弾が当たったときなど）
    /// </summary>
    public virtual void Disable() { }

    /// <summary>
    /// 通常モード終了時にエリアマネージャーのイベント経由で呼ばれる
    /// 数秒後に非常時モードに切り替わる
    /// </summary>
    protected virtual void ExitNormalMode() { }

    /// <summary>
    /// 非常時モード移行時にエリアマネージャーのイベント経由で呼ばれる
    /// </summary>
    protected virtual void EnterEmergencyMode() { }

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    protected virtual void OnDestroy()
    {
        // TODO: manager.OnEmergencyMode -= EnterEmergencyMode;
    }
}