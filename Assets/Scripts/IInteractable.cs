/// <summary>
/// プレイヤーがインタラクト可能なオブジェクトが実装するインターフェース
/// 各対象のPresenter層が実装する想定
/// </summary>
public interface IInteractable
{
    /// <summary>ハイライト状態の切り替え（マテリアルのActiveなどに反映する）</summary>
    void SetHighlighted(bool active);

    /// <summary>インタラクト実行</summary>
    void Interact();
}