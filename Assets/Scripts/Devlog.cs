using UnityEngine;

/// <summary>
/// エディタ・開発ビルドのみログを出力するユーティリティ
/// DEVELOPMENT_BUILD シンボルはUnityがDevelopment Buildチェック時に自動定義する
/// </summary>
public static class DevLog
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Warning(string message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Error(string message)
    {
        Debug.LogError(message);
    }
}
