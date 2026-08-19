using UnityEngine;

public class EntityPresenter : MonoBehaviour
{
    private protected EntityModel _model;
    public EntityModel Model => _model;

    [SerializeField] private protected EntityView _view;

    private bool _initialized;

    /// <summary>
    /// pathfinderはNavGraphScanner.StageContextから取得してPresenterに渡す
    /// ナビゲーションが不要なエンティティ（プレイヤーなど）はnullを渡す
    /// </summary>
    public void Init(EntityStatsData data, NavPathfinder pathfinder = null)
    {
        _view.Init(pathfinder);
        _model = data.CreateModel(_view);
        Bind();

        _initialized = true;
    }

    public virtual void Bind()
    {
        //_model.OnRequestMove += _view.Move;
        _model.OnHPDamaged += _view.OnHPDamaged;
        _model.OnArmorDamaged += _view.OnShieldDamaged;
        _model.OnShieldBreak += _view.OnShieldBreak;
        _model.OnDeath += _view.OnDeath;
    }

    private void Update()
    {
        if(_initialized)
        {
            _model.Tick(Time.deltaTime, _view.Position);
            _view.Tick(Time.deltaTime);
        }
    }
}
