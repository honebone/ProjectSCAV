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
        // TODO: manager.OnEmergencyMode += EnterEmergencyMode;
        // TODO: manager.OnXxx += Xxx;
    }

    // -------------------------------------------------------
    // イベント
    // -------------------------------------------------------

    /// <summary>
    /// 故障時に呼ばれる（弾が当たったときなど）
    /// </summary>
    public virtual void Disable() { }

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