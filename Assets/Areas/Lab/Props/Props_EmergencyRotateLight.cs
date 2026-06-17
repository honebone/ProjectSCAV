using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Props_EmergencyRotateLight : Props_Light
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private List<Light2D> _lights;
    [SerializeField] Transform _rotateTF;
    [SerializeField] float _rotateSpeed;

    private Material _targetMaterial;
    private bool _emergency;

    void Start()
    {
        _targetMaterial = _spriteRenderer.material;

        _targetMaterial.SetInt("_Emit", 0);
        _lights.ForEach(l => l.enabled = false);
    }

    private void Update()
    {
        if (_emergency)
        {
            _rotateTF.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
        }
    }

    protected override void EnterEmergencyMode()
    {
        _targetMaterial.SetInt("_Emit", 1);
        _lights.ForEach(l => l.enabled = true);

        _emergency = true;
    }

    protected override void SetIntensity(float value)
    {

    }
}
