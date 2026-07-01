using UnityEngine;
using System;

public class Layer2Controller : MonoBehaviour
{
    [SerializeField] private InventoryPresenter _playerInventoryUI;
    [SerializeField] private UIPanel _loadoutUI;
    [SerializeField] private InventoryPresenter _lootInventoryUI;
    [SerializeField] private UIPanel _shopUI;
    [SerializeField] private UIPanel _questsUI;
    [SerializeField] private UIPanel _logUI;

    private readonly Layer2State _state = new Layer2State();
    public Layer2State State => _state;

    // OpenLoot/OpenShopで渡されたモデルを一時保持
    private InventoryModel _pendingLootModel;
    private InventoryModel _playerInventoryModel;

    private void Awake()
    {
        _state.OnStateChanged += OnStateChanged;
    }

    public void Tick(IUIInputGetter input)
    {
        if (input.InventoryToggle) _state.ToggleInventory();
        if (input.QuestsToggle) _state.ToggleQuests();
        if (input.LogToggle) _state.ToggleLog();
        if (input.CloseUI) _state.CloseAll();
    }

    /// <summary>初期化時にPlayerModelから取得したInventoryModelを渡す</summary>
    public void Init(InventoryModel playerInventory)
    {
        _playerInventoryModel = playerInventory;
    }

    public void OpenLoot(InventoryModel lootInventory)
    {
        _pendingLootModel = lootInventory;
        _state.OpenLootInventory();
    }

    private void OnStateChanged(Layer2UI prev, Layer2UI next)
    {
        bool showPlayerInv = _state.ShowPlayerInventory;

        // プレイヤーインベントリの開閉
        if (showPlayerInv && !_playerInventoryUI.Active) _playerInventoryUI.Open(_playerInventoryModel);
        else if (!showPlayerInv && _playerInventoryUI.Active) _playerInventoryUI.Close();

        _loadoutUI.SetPanelActive(showPlayerInv);

        // ルートインベントリの開閉
        bool showLoot = next == Layer2UI.LootInventory;
        if (showLoot) _lootInventoryUI.Open(_pendingLootModel);
        else if (prev == Layer2UI.LootInventory) _lootInventoryUI.Close();

        // ショップ・案件・ログも同様にSetActiveのみ（Presenter不要なら省略）
        _shopUI.SetPanelActive(next == Layer2UI.Shop);
        _questsUI.SetPanelActive(next == Layer2UI.Quests);
        _logUI.SetPanelActive(next == Layer2UI.Log);
    }
}
/// <summary>
/// レイヤー2全体の開閉状態をpure C#で管理する
/// 常に最大1つの状態のみアクティブ
/// </summary>
public class Layer2State
{
    private Layer2UI _current = Layer2UI.None;
    public Layer2UI Current => _current;

    public event Action<Layer2UI, Layer2UI> OnStateChanged; // (前の状態, 新しい状態)

    /// <summary>Tabキー：PlayerInventoryの開閉トグル</summary>
    public void ToggleInventory()
    {
        if (_current == Layer2UI.PlayerInventory) SetState(Layer2UI.None);
        else SetState(Layer2UI.PlayerInventory);
    }

    /// <summary>Tキー：Questsの開閉トグル</summary>
    public void ToggleQuests()
    {
        if (_current == Layer2UI.Quests) SetState(Layer2UI.None);
        else SetState(Layer2UI.Quests);
    }

    /// <summary>Yキー：Logの開閉トグル</summary>
    public void ToggleLog()
    {
        if (_current == Layer2UI.Log) SetState(Layer2UI.None);
        else SetState(Layer2UI.Log);
    }

    /// <summary>ルートボックス・倉庫にインタラクトしたとき</summary>
    public void OpenLootInventory()
    {
        SetState(Layer2UI.LootInventory);
    }

    /// <summary>ショップにインタラクトしたとき</summary>
    public void OpenShop()
    {
        SetState(Layer2UI.Shop);
    }

    /// <summary>Esc/Pキー：レイヤー2を閉じる</summary>
    public void CloseAll()
    {
        SetState(Layer2UI.None);
    }

    public bool IsOpen => _current != Layer2UI.None;

    /// <summary>PlayerInventory + Loadoutが表示されるべきか</summary>
    public bool ShowPlayerInventory =>
        _current == Layer2UI.PlayerInventory
        || _current == Layer2UI.LootInventory
        || _current == Layer2UI.Shop;

    private void SetState(Layer2UI next)
    {
        if (_current == next) return;
        Layer2UI prev = _current;
        _current = next;
        OnStateChanged?.Invoke(prev, next);
    }
}

public enum Layer2UI
{
    None,
    PlayerInventory,   // プレイヤーインベントリ+ロードアウト（常にセット）
    LootInventory,     // +プレイヤーインベントリ+ロードアウト
    Shop,              // +プレイヤーインベントリ+ロードアウト
    Quests,            // 単独
    Log,               // 単独
}