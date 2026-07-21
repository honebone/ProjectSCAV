using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EntityWorldText : MonoBehaviour
{
    [SerializeField] private float _scaleDur;
    [SerializeField] private float _delay;
    [SerializeField] private float _rotDur;
    [SerializeField] private Vector2 _offseth;
    [SerializeField] private Vector2 _offsetv;
    [SerializeField] private Vector2 _popIntensityRange;
    [SerializeField] private float _gravity;
    [SerializeField] private Vector2 _popAngleRange;
    [SerializeField] private TextMeshProUGUI _damageText;

    Vector2 _popvec;
    public void Init(string str)
    {
        Vector2 offset = new Vector2(_offseth.Range(), _offsetv.Range());
        transform.Translate(offset);

        float popIntensity= _popIntensityRange.Range();
        float angle = _popAngleRange.Range() * Mathf.Deg2Rad;
        _popvec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * popIntensity;

        _damageText.text = str;
        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, _scaleDur).SetEase(Ease.OutCubic));
        seq.AppendInterval(_delay);
        seq.Append(transform.DORotate(new Vector3(90, 0, 0), _rotDur).SetEase(Ease.InElastic).OnComplete(() => { Destroy(gameObject); }));
    }

    private void Update()
    {
        transform.Translate(_popvec * Time.deltaTime);
        _popvec.y -= _gravity * Time.deltaTime;
    }
}
