/// <summary>
/// エリア全体の状態を管理する pure C# クラス
/// 現在は LootGeneratorModel を保持するのみだが、
/// 今後は敵管理・タイマーなどの責務を追加予定
/// </summary>
public class AreaManagerModel
{
    private AreaData _areaData;
    private readonly LootGeneratorModel _lootGenerator;
    public LootGeneratorModel LootGenerator => _lootGenerator;

    public AreaManagerModel(AreaData areaData)
    {
        _lootGenerator = new LootGeneratorModel();
    }
}
