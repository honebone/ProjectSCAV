using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer1Controller : MonoBehaviour
{
    [SerializeField] private UIPanel _menuUI;
    [SerializeField] private UIPanel _settingsUI;

    private readonly Layer1State _state = new Layer1State();

    private void Awake()
    {
        _state.OnStateChanged += OnStateChanged;
    }

    /// <param name="layer2IsOpen">Layer2Controller.State.IsOpenを渡す</param>
    public void Tick(IUIInputGetter input, bool layer2IsOpen)
    {
        if (input.CloseUI)
        {
            // レイヤー2を開いていないときのみメニューを開閉
            if (!layer2IsOpen)
            {
                if (_state.Current == Layer1UI.None) _state.SetState(Layer1UI.Menu);
                else _state.SetState(Layer1UI.None);
            }
        }
    }

    /// <summary>メニュー画面で設定ボタンを押したとき</summary>
    public void OpenSettings() => _state.SetState(Layer1UI.Settings);

    /// <summary>設定画面で戻るボタンを押したとき</summary>
    public void BackToMenu() => _state.SetState(Layer1UI.Menu);

    private void OnStateChanged(Layer1UI next)
    {
        _menuUI.SetPanelActive(next == Layer1UI.Menu);
        _settingsUI.SetPanelActive(next == Layer1UI.Settings);
    }
}

public enum Layer1UI { None, Menu, Settings }

public class Layer1State
{
    private Layer1UI _current = Layer1UI.None;
    public Layer1UI Current => _current;
    public event Action<Layer1UI> OnStateChanged;

    public void SetState(Layer1UI next)
    {
        if (_current == next) return;
        _current = next;
        OnStateChanged?.Invoke(next);
    }
}