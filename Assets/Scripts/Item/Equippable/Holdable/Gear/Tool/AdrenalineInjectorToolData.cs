using UnityEngine;

/// <summary>
/// アドレナリンインジェクターのScriptableObject（パッシブ効果フレームワークの検証用サンプル実装）
/// 手に持っている間、いずれかの銃のリロードが完了するたびに自身へEffectActionDataで設定した効果（バフ付与等）を発動する
/// </summary>
[CreateAssetMenu(menuName = "Item/AdrenalineInjectorToolData")]
public class AdrenalineInjectorToolData : ToolData
{
    [SerializeField, Header("リロード完了時に自身へ発火する効果")] private EffectActionData _onReloadEffect;
    public EffectActionData OnReloadEffect => _onReloadEffect;

    public override ItemModel CreateModel() { return new AdrenalineInjectorToolModel(this); }
}
