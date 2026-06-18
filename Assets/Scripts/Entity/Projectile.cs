using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _groundLayer;

    private FireParams _fireParams;

    public void Init(FireParams fireParams)
    {
        _fireParams = fireParams;

        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        Vector2 velocity = rotation * Vector2.right * fireParams.Snapshot.BulletSpeed;
        _rb.velocity = velocity;

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
        DevLog.Log("ok");
        if (((1 << collision.gameObject.layer) & _groundLayer) != 0)
        {
            DevLog.Log("ok2");
            Destroy(gameObject);
        }
    }

    //’…’eŽž‚Ìˆ—AÁ–Å‚Ìˆ—‚È‚Ç...
}
