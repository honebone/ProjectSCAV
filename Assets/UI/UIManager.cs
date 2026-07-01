using UnityEngine;

public class UIManager : MonoBehaviour, IUIInputGetter
{
    [SerializeField] private Layer1Controller _layer1;
    [SerializeField] private Layer2Controller _layer2;

    // IUIInputGetter実装（前回提示分に追加）
    public bool InventoryToggle { get; private set; }
    public bool QuestsToggle { get; private set; }
    public bool LogToggle { get; private set; }
    public bool CloseUI { get; private set; }
    public bool QuickMove { get; private set; }
    public bool Discard { get; private set; }

    public void Init(InventoryModel playerInventory)
    {
        _layer2.Init(playerInventory);
    }

    private void Update()
    {
        InventoryToggle = Input.GetKeyDown(KeyCode.Tab);
        QuestsToggle = Input.GetKeyDown(KeyCode.T);
        LogToggle = Input.GetKeyDown(KeyCode.Y);
        CloseUI = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
        QuickMove = Input.GetMouseButtonDown(0);
        Discard = Input.GetKeyDown(KeyCode.F);

        bool layer2WasOpen = _layer2.State.IsOpen;

        // Esc/Pはレイヤー2優先：開いていればレイヤー2のCloseAllのみ処理
        if (CloseUI && layer2WasOpen)
        {
            _layer2.State.CloseAll();
        }
        else
        {
            _layer2.Tick(this);
            _layer1.Tick(this, _layer2.State.IsOpen);
        }
    }
}
