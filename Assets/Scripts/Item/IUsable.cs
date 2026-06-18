/// <summary>
/// 手に持って使用できるアイテムのインターフェース
/// 銃・消耗品・道具が実装する
/// </summary>
public interface IUsable
{
    /// <summary>使用にかかる時間（秒）</summary>
    float UseTime { get; }

    /// <summary>使用開始</summary>
    void Use(EntityModel user);

    /// <summary>使用終了（長押し系アイテムの解放など）</summary>
    void StopUsing(EntityModel user);
}
