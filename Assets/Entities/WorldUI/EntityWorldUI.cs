using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityWorldUI : MonoBehaviour
{
    [SerializeField] private UIPanel _sliderUI;
    [SerializeField] private Image _sliderFill;
    [SerializeField] private Animator _sliderImageAnim;
    public void SpawnText(string str)
    {
        Instantiate(Constants.Instance.WorldTextPrefab, transform).Init(str);
    }
    public void SpawnImage(GameObject image)
    {
        Instantiate(image, transform);
    }

    public void SetSliderUI(string value)
    {
        _sliderUI.SetPanelActive(true);
        _sliderFill.fillAmount = 0;
        _sliderImageAnim.SetTrigger(value);
    }

    public void SetSliderFill(float value,float max)
    {
        _sliderFill.fillAmount = 1f - value / max;
    }

    public void ResetSliderUI()
    {
        _sliderImageAnim.SetTrigger("Idle");
        _sliderFill.fillAmount = 0;
        _sliderUI.SetPanelActive(false);
    }
}
