/// <summary>
/// スポーン完了後のステージ全体のデータを保持するコンテキスト
/// GeneratedLayout（マップ構造）と NavGraph（ナビゲーション）をまとめる
/// 将来的に敵リスト・脱出ポイント確定結果なども追加予定
/// </summary>
public class StageContext
{
    private readonly GeneratedLayout _layout;
    private readonly NavGraph _navGraph;

    public GeneratedLayout Layout => _layout;
    public NavGraph NavGraph => _navGraph;

    public StageContext(GeneratedLayout layout, NavGraph navGraph)
    {
        _layout = layout;
        _navGraph = navGraph;
    }
}