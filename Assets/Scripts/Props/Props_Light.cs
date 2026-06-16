using UnityEngine;

/// <summary>
/// 照明系Propsの基底クラス
/// </summary>
public abstract class Props_Light : Props
{
    // -------------------------------------------------------
    // 明るさ設定
    // -------------------------------------------------------

    /// <summary>明るさを設定する　0で消灯、1で最大輝度</summary>
    /// <param name="value">0～1</param>
    protected abstract void SetIntensity(float value);
}