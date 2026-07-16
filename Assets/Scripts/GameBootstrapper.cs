using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 各種データベースのロードを担う静的クラス
/// MonoBehaviourではないためシーンに配置する必要がなく、
/// RuntimeInitializeOnLoadMethodによりどのシーンから再生してもロードが保証される
/// （テストプレイ時に任意のゲームシーンから直接再生しても動作する）
///
/// データベースを使うクラスは、使用前に必ず WaitForReadyAsync() を呼ぶこと
/// </summary>
public static class GameBootstrapper
{
    public static ItemDatabaseModel ItemDatabase { get; private set; }
    public static bool IsReady { get; private set; }

    private static Task _loadingTask;

    /// <summary>
    /// ゲーム起動時（最初のシーンがロードされる前）に自動的に呼ばれる
    /// エディタでPlayを押した場合も、開いているシーンに関係なく実行される
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnRuntimeInitialize()
    {
        if (_loadingTask != null) return; // 二重実行防止
        _loadingTask = LoadAllAsync();
    }

    private static async Task LoadAllAsync()
    {
        ItemDatabase = await ItemDatabaseLoader.LoadAsync();
        // 今後 GunDatabase 等を追加する場合はここで同様にロードする

        IsReady = true;
        DevLog.Log("[GameBootstrapper] 全データベースのロードが完了しました");
    }

    /// <summary>
    /// ロード完了を待つ。データベースを使う全てのシーン/クラスは
    /// 使用前に必ずこれを呼ぶこと。ロード済みなら即座に完了する
    /// </summary>
    public static async Task WaitForReadyAsync()
    {
        // RuntimeInitializeOnLoadMethodが未実行だった場合の保険（Domain Reload無効時など）
        if (_loadingTask == null) _loadingTask = LoadAllAsync();
        await _loadingTask;
    }
}