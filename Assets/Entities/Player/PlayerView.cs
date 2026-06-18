using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class PlayerView : EntityView,IInputGetter,IItemVisualizer
{
    [SerializeField] private Light2D _headLight;
    [SerializeField] private Transform _headLight_tf;

    [SerializeField] private HoldingItemView _holdingItemView;
    public HoldingItemView HoldingItemView => _holdingItemView;
    // -------------------------------------------------------
    // IInputGetter : キーバインド設定
    // -------------------------------------------------------

    [Header("Key Bindings")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _interactKey = KeyCode.F;
    [SerializeField] private KeyCode _useKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _jetpackKey = KeyCode.Space; // ジャンプと同キーが一般的だが変更可能

    // -------------------------------------------------------
    // IInputGetter : キャッシュ
    // -------------------------------------------------------

    // Tickで毎フレーム更新し、Model側はキャッシュ値を読む
    // Input.GetKeyDown/Upはその瞬間のみtrueになるため、Tickで必ず取得する

    private float _moveAxis;
    private bool _jumpDown;
    private bool _jumpHold;
    private bool _jumpUp;
    private bool _interactDown;
    private bool _useDown;
    private bool _useHold;
    private bool _useUp;
    private bool _swapItem_next;
    private bool _swapItem_back;
    private bool _jetpackDown;
    private bool _jetpackHold;
    private bool _jetpackUp;
    private Vector2 _mousePos;

    private float _wheel;

    // -------------------------------------------------------
    // IInputGetter : プロパティ
    // -------------------------------------------------------

    public float MoveAxis => _moveAxis;
    public bool JumpDown => _jumpDown;
    public bool JumpHold => _jumpHold;
    public bool JumpUp => _jumpUp;
    public bool InteractDown => _interactDown;
    public bool UseDown => _useDown;
    public bool UseHold => _useHold;
    public bool UseUp => _useUp;
    public bool SwapItem_Next => _swapItem_next;
    public bool SwapItem_Back => _swapItem_back;
    public bool JetpackDown => _jetpackDown;
    public bool JetpackHold => _jetpackHold;
    public bool JetpackUp => _jetpackUp;
    public Vector2 MousePos => _mousePos;

    public override void Init(NavPathfinder pathfinder)
    {
        base.Init(pathfinder);
        _holdingItemView.init(this);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        // 水平入力 (-1 / 0 / 1)
        _moveAxis = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) _moveAxis -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) _moveAxis += 1f;

        // ジャンプ
        _jumpDown = Input.GetKeyDown(_jumpKey);
        _jumpHold = Input.GetKey(_jumpKey);
        _jumpUp = Input.GetKeyUp(_jumpKey);

        // インタラクト
        _interactDown = Input.GetKeyDown(_interactKey);

        // 使用
        _useDown = Input.GetKeyDown(_useKey);
        _useHold = Input.GetKey(_useKey);
        _useUp = Input.GetKeyUp(_useKey);

        // ジェットパック
        _jetpackDown = Input.GetKeyDown(_jetpackKey);
        _jetpackHold = Input.GetKey(_jetpackKey);
        _jetpackUp = Input.GetKeyUp(_jetpackKey);

        // マウスカーソル
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _wheel += Input.mouseScrollDelta.y;
        if (_wheel < 0)
        {
            _swapItem_next = true;
            _wheel = 0;
        }
        else if (_wheel > 0)
        {
            _swapItem_back = true;
            _wheel = 0;
        }
        else
        {
            _swapItem_next = false;
            _swapItem_back = false;
        }
    }

    public override void Look(Vector2 lookAt, float angle, float range)
    {
        Vector2 direction = new Vector2(
            lookAt.x - transform.position.x,
            lookAt.y - transform.position.y
        );

        float lookAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _headLight_tf.rotation = Quaternion.Euler(0, 0, lookAngle);
        _headLight.pointLightInnerAngle = angle/2;
        _headLight.pointLightOuterAngle = angle;
        _headLight.pointLightOuterRadius = range;

        _holdingItemView.UpdateAim(lookAt);
    }
    public void OnItemHeld(HoldableItemModel model)
    {
        _holdingItemView.OnItemHeld(model);
    }
    public void UpdateAim(Vector2 lookAt)
    {
        _holdingItemView.UpdateAim(lookAt);
    }
}
