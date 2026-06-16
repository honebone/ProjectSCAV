using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップ生成ロジック / pure C#
/// UnityのAPIに依存しない（UnityEngine.Randomを除く）
/// </summary>
public class MapGeneratorModel
{
    private const int GridSize = 9;
    private readonly int _targetRoomCount;

    public MapGeneratorModel(int targetRoomCount)
    {
        _targetRoomCount = targetRoomCount;
    }

    public GeneratedLayout Generate(RoomDatabase db)
    {
        GeneratedLayout layout = new GeneratedLayout(GridSize);
        Queue<(Vector2Int pos, Direction requiredDir)> queue = new();
        HashSet<Vector2Int> queued = new();

        Vector2Int start = layout.StartPos;
        layout.Place(start, db.StartRoom);
        //DevLog.Log($"[MapGen] 配置: ({start.x},{start.y}) スタート部屋 {db.StartRoom.PassagePattern}");

        EnqueueOpenDirections(db.StartRoom, start, queue, queued, layout);

        while (queue.Count > 0)
        {
            (Vector2Int pos, Direction reqDir) = queue.Dequeue();

            if (!layout.InBounds(pos))
            {
                //DevLog.Log($"[MapGen] スキップ(範囲外): ({pos.x},{pos.y})");
                continue;
            }

            if (layout.Get(pos) != null)
            {
                //DevLog.Log($"[MapGen] スキップ(配置済み): ({pos.x},{pos.y})");
                continue;
            }

            bool isTerminating = layout.RoomCount >= _targetRoomCount - 2;

            RoomData selected = PickRoom(db, layout, pos, reqDir, isTerminating);

            if (selected == null)
            {
                //DevLog.Warning($"[MapGen] スキップ(整合する部屋なし): ({pos.x},{pos.y}) 要求方向={reqDir} 行き止まりモード={isTerminating}");
                continue;
            }

            layout.Place(pos, selected);
            //DevLog.Log($"[MapGen] 配置: ({pos.x},{pos.y}) {selected.PassagePattern} 行き止まりモード={isTerminating} 合計={layout.RoomCount}部屋");

            if (!isTerminating)
                EnqueueOpenDirections(selected, pos, queue, queued, layout);
        }

        DevLog.Log($"[MapGen] 生成完了: {layout.RoomCount}部屋");
        return layout;
    }

    private RoomData PickRoom(
        RoomDatabase db, GeneratedLayout layout,
        Vector2Int pos, Direction reqDir, bool isTerminating)
    {
        bool? needN = null, needS = null, needE = null, needW = null;

        DirectionUtility.ApplyConstraint(reqDir, true, ref needN, ref needS, ref needE, ref needW);

        if (isTerminating)
        {
            if (needN == null) needN = false;
            if (needS == null) needS = false;
            if (needE == null) needE = false;
            if (needW == null) needW = false;
        }

        foreach (Direction dir in DirectionUtility.All)
        {
            Vector2Int nPos = pos + DirectionUtility.ToOffset(dir);
            if (!layout.InBounds(nPos))//先が範囲外ならその方向には通路を作らない
            {
                DirectionUtility.ApplyConstraint(dir, false, ref needN, ref needS, ref needE, ref needW);
                continue;
            }

            RoomData neighbor = layout.Get(nPos);
            if (neighbor == null) continue;

            bool neighborFacesMe = neighbor.HasPassage(DirectionUtility.Opposite(dir));
            DirectionUtility.ApplyConstraint(dir, neighborFacesMe, ref needN, ref needS, ref needE, ref needW);
        }

        return db.GetFiltered(needN, needS, needE, needW);
    }

    private void EnqueueOpenDirections(
        RoomData room, Vector2Int pos,
        Queue<(Vector2Int, Direction)> queue,
        HashSet<Vector2Int> queued,
        GeneratedLayout layout)
    {
        foreach (Direction dir in DirectionUtility.All)
        {
            if (!room.HasPassage(dir)) continue;//この方向に通路がないならスキップ

            Vector2Int next = pos + DirectionUtility.ToOffset(dir);

            if (!layout.InBounds(next)) continue;//前方がグリッド外ならスキップ
            if (layout.Get(next) != null) continue;//前方にすでに部屋があるならスキップ
            if (queued.Contains(next)) continue;//前方を生成するキューがすでにあるならスキップ

            queue.Enqueue((next, DirectionUtility.Opposite(dir)));
            queued.Add(next);
        }
    }
}