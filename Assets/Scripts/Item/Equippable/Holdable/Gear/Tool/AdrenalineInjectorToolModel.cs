/// <summary>
/// アドレナリンインジェクターのModel（パッシブ効果フレームワークの検証用サンプル実装）
/// 「自身への作用トリガー」カテゴリの例：リロード完了イベントを受けてEffectActionDataから作った効果を発動する
///
/// LoadoutModel.OnAnyGunReloadCompleted（銃個別ではなく所有者に紐づく安定したハブ）を購読することで、
/// 武器を持ち替えても購読が途切れない
/// </summary>
public class AdrenalineInjectorToolModel : ToolModel
{
    private readonly AdrenalineInjectorToolData _data;
    private EntityModel _owner;

    public AdrenalineInjectorToolModel(AdrenalineInjectorToolData data) : base(data)
    {
        _data = data;
    }

    public override void OnApply(EntityModel owner)
    {
        base.OnApply(owner);
        _owner = owner;
        if (owner is ILoadoutable loadoutable) loadoutable.Loadout.OnAnyGunReloadCompleted += HandleReloadCompleted;
    }

    public override void OnRemove(EntityModel owner)
    {
        base.OnRemove(owner);
        if (owner is ILoadoutable loadoutable) loadoutable.Loadout.OnAnyGunReloadCompleted -= HandleReloadCompleted;
        _owner = null;
    }

    private void HandleReloadCompleted(float reloadTime)
    {
        if (_owner == null) return;
        _owner.Resolve(_data.OnReloadEffect.Create(_owner));
    }
}
