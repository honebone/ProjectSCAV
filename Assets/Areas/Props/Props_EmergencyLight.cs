using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Props_EmergencyLight : Props_Light
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Light2D _light;

    private Material _targetMaterial;

    void Start()
    {
        _targetMaterial = _spriteRenderer.material;

        _targetMaterial.SetInt("_Emit", 0);
        _light.enabled = false;
    }

    protected override void EnterEmergencyMode()
    {
        _targetMaterial.SetInt("_Emit", 1);
        _light.enabled = true;
    }

    protected override void SetIntensity(float value)
    {
        
    }
}
