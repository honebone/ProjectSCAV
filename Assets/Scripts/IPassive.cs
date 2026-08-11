/// <summary>
/// 装備・付与されている間パッシブ効果を持つもののインターフェース
/// インプラント・道具（ToolModel）・バフ（BuffModel）が実装する
/// </summary>
public interface IPassive
{
    /// <summary>効果が付与されたとき（Entityのステータスへの補正付与など）</summary>
    void OnApply(EntityModel owner);

    /// <summary>効果が取り除かれたとき（補正の除去など）</summary>
    void OnRemove(EntityModel owner);
}
