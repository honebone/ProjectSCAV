using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectController : MonoBehaviour
{
    [SerializeField] private EntityWorldUI _worldUI;
    [SerializeField] private ParticleSystem _par_onShieldDMG;
    [SerializeField] private ParticleSystem _par_onHPDMG;
    [SerializeField] private Transform _bodyParts;
    [SerializeField] private float _explodeBodyPartsForce = 5f;

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

        Destroy(gameObject, 10f);
    }
}
