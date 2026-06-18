/// <summary>
/// 装備中にパッシブ効果を持つアイテムのインターフェース
/// インプラント・道具（ToolModel）が実装する
/// </summary>
public interface IPassive
{
    /// <summary>装備したとき（Entityのステータスへの補正付与など）</summary>
    void OnEquip(EntityModel owner);

    /// <summary>取り外したとき（補正の除去など）</summary>
    void OnUnequip(EntityModel owner);
}
