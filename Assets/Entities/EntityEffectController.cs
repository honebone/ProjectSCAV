using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectController : MonoBehaviour
{
    [SerializeField] private EntityWorldUI _worldUI;
    [SerializeField] private ParticleSystem _par_onShieldDMG;
    [SerializeField] private ParticleSystem _par_onHPDMG;
    [SerializeField] private ParticleSystem _par_onDeath;
    [SerializeField] private Transform _bodyParts;
    [SerializeField] private float _explodeBodyPartsForce = 5f;
    [SerializeField] private int _sparkEmissionsOnDeath;
    [SerializeField] private int _partsEmissionsOnDeath;
    [SerializeField] private float _hitStopTimeScale;
    [SerializeField] private float _hitStopDelay;
    [SerializeField] private float _hitStopDuration;

    public EntityWorldUI WorldUI => _worldUI;
    public ParticleSystem Par_onShieldDMG => _par_onShieldDMG;
    public ParticleSystem Par_onHPDMG => _par_onHPDMG;

    public void DeathEffect()
    {
        _bodyParts.gameObject.SetActive(true);
        Vector3 origin = transform.position;
        foreach(var part in _bodyParts.GetComponentsInChildren<Rigidbody2D>())
        {
            Vector3 dir = (part.GetComponent<Collider2D>().bounds.center - origin).normalized;
            part.AddForce(dir * _explodeBodyPartsForce, ForceMode2D.Impulse);
        }

        _worldUI.SpawnImage(Constants.Instance.VE_Die);
        _par_onHPDMG.Emit(_sparkEmissionsOnDeath);
        _par_onDeath.Emit(_partsEmissionsOnDeath);
        TimeScaleManager.SetScaleThenRecover(_hitStopTimeScale, _hitStopDelay, _hitStopDuration);
        
        Destroy(gameObject, 10f);
    }
}
