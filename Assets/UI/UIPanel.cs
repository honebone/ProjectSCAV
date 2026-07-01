using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [SerializeField]private protected CanvasGroup _canvas;
    private bool _active;
    public bool Active => _active;

    public void SetPanelActive(bool active)
    {
        _active = active;
        _canvas.alpha = active ? 1 : 0;
        _canvas.blocksRaycasts = active;
        _canvas.interactable = active;
    }

    [ContextMenu("Toggle Active")]
    public void ToggleActiveFromInspector()
    {
        SetPanelActive(!_active);
    }
}
