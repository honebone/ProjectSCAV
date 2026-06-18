using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingItemView : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;  // 手の位置
    [SerializeField] private Transform _lookPivot;  // 回転の中心

    private IProjectileSpawner _projectileSpawner;

    private GameObject _currentItemObject;
    private IItemVisual _currentVisual;   // 生成したItemObjectが持つインターフェース

    public void init(IProjectileSpawner projectileSpawner)
    {
        _projectileSpawner = projectileSpawner;
    }

    /// <summary>
    /// LoadoutModelのスロット切り替え時にPresenterから呼ばれる
    /// </summary>
    public void OnItemHeld(HoldableItemModel model)
    {
        // 前のオブジェクトの後片付け
        if (_currentVisual != null)
        {
            _currentVisual.Unsubscribe();
            Destroy(_currentItemObject);
        }

        if (model == null) return;

        // ItemDataに紐付いたPrefabを生成
        _currentItemObject = Instantiate(model.Prefab, _holdPoint);
        _currentVisual = _currentItemObject.GetComponent<IItemVisual>();
        _currentVisual.Init(_projectileSpawner);
        _currentVisual?.Subscribe(model);
    }

    /// <summary>毎フレームPresenterから呼ぶ（向きの更新）</summary>
    public void UpdateAim(Vector2 lookAt)
    {
        if (_lookPivot == null) return;

        Vector2 direction = new Vector2(
            lookAt.x - _lookPivot.position.x,
            lookAt.y - _lookPivot.position.y
        );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = direction.x < 0 ? 180 - angle : angle;


        // 左向きのときスプライトを上下反転
        //Vector3 scale = _lookPivot.localScale;
        //scale.y = direction.x < 0 ? -1 : 1;
        //_lookPivot.localScale = scale;
        float angleY = direction.x < 0 ? 180 : 0;

        _lookPivot.rotation = Quaternion.Euler(0, angleY, angle);
    }
}