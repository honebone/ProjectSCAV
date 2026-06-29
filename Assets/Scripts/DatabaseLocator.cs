using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DatabaseLocator")]
public class DatabaseLocator : ScriptableObject
{
    private static DatabaseLocator _instance;
    public static DatabaseLocator Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<DatabaseLocator>("DatabaseLocator");
            return _instance;
        }
    }

    [SerializeField] private ItemDatabase _itemDatabase;
    //[SerializeField] private EnemyDatabase _enemyDatabase;
    //[SerializeField] private AreaDatabase _roomDatabase;

    public ItemDatabase ItemDatabase => _itemDatabase;
    //public EnemyDatabase EnemyDatabase => _enemyDatabase;
    //public RoomDatabase AreaDatabase => _roomDatabase;
}
