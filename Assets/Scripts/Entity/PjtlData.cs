using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PjtlData")]
public class PjtlData : ScriptableObject
{
    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private int _pelletPerShot = 1;

    [SerializeField] private float _spread;
    [SerializeField] private bool _equidistant;

    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _bulletLifeTime;
    [SerializeField] private int _penetration;

    public GameObject BulletPrefab => _bulletPrefab;
    public int PelletPerShot => _pelletPerShot;
    public float Spread => _spread;
    public bool Equidistant => _equidistant;
    public float BulletSpeed => _bulletSpeed;
    public float BulletLifetime => _bulletLifeTime;
    public int Penetration => _penetration;
}
