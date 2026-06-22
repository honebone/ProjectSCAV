using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _entityLayer;
    [SerializeField] private LayerMask _propsLayer;

    private FireParams _fireParams;

    private List<EntityModel> _hitList=new List<EntityModel>();
    private int _penetrations;

    public void Init(FireParams fireParams)
    {
        _fireParams = fireParams;

        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        Vector2 velocity = rotation * Vector2.right * fireParams.Snapshot.BulletSpeed;
        _rb.velocity = velocity;

        _penetrations = _fireParams.Snapshot.Penetration;

        Destroy(gameObject, fireParams.Snapshot.BulletLifetime);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    DevLog.Log("ok");
    //    if (((1 << other.gameObject.layer) & _groundLayer) != 0)
    //    {
    //        DevLog.Log("ok2");
    //        Destroy(gameObject);
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _propsLayer) != 0)
        {
            collision.gameObject.GetComponent<Props>()?.Disable();
        }

        if (((1 << collision.gameObject.layer) & _entityLayer) != 0)
        {
            EntityModel target = collision.gameObject.GetComponent<EntityPresenter>().Model;
            if (target != null && target.Alive && _fireParams.TargetFaction.Contains(target.Faction) && !_hitList.Contains(target))
            {
                _hitList.Add(target);

                target.Damage(15);

                if (_penetrations == 0) Destroy(gameObject);
                else _penetrations--;
            }
        }

        if (((1 << collision.gameObject.layer) & _groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
