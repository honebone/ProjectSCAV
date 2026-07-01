using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// RoomDataのデータベース
/// Resources/MapGen/RoomDatabase.asset として配置することで
/// RoomDatabaseAPI.Instance 経由でどこからでもアクセスできる
/// </summary>
[CreateAssetMenu(menuName = "MapGen/RoomDatabase", fileName = "RoomDatabase")]
public class RoomDatabase : ScriptableObject
{
    [SerializeField] private RoomData _startRoom;
    [SerializeField] private List<RoomData> _allRooms;

    public RoomData StartRoom => _startRoom;

    // -------------------------------------------------------
    // シングルトンAPI
    // -------------------------------------------------------

    //private const string ResourcePath = "MapGen/RoomDataBase";
    //private static RoomDatabase _instance;

    //public static RoomDatabase Instance
    //{
    //    get
    //    {
    //        if (_instance != null) return _instance;

    //        _instance = Resources.Load<RoomDatabase>(ResourcePath);

    //        if (_instance == null)
    //        {
    //            DevLog.Error($"[RoomDatabase] Resources/{ResourcePath}.asset が見つかりません。");
    //        }

    //        return _instance;
    //    }
    //}

    // -------------------------------------------------------
    // クエリ
    // -------------------------------------------------------

    /// <summary>
    /// 通路条件に合う部屋をウェイト付きランダムで返す
    /// null = どちらでもよい
    /// 条件を満たす部屋が存在しない場合は null を返す
    /// </summary>
    public RoomData GetFiltered(bool? n, bool? s, bool? e, bool? w)
    {
        List<RoomData> candidates = _allRooms.Where(r =>
            (n == null || r.HasNorth == n) &&
            (s == null || r.HasSouth == s) &&
            (e == null || r.HasEast == e) &&
            (w == null || r.HasWest == w)
        ).ToList();

        if (candidates.Count == 0) return null;

        //float total = candidates.Sum(r => r.Weight);
        //float roll = Random.Range(0f, total);
        //float cum = 0f;

        //foreach (RoomData room in candidates)
        //{
        //    cum += room.Weight;
        //    if (roll < cum) return room;
        //}

        //// 浮動小数点の誤差対策
        //return candidates[^1];
        return candidates[candidates.Select(r => r.Weight).ToList().ChoiceWithWeight()];
    }
}