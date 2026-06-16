using UnityEngine;

public enum Direction
{
    North,
    South,
    East,
    West,
}

public static class DirectionUtility
{
    /// <summary>反対方向を返す</summary>
    public static Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => throw new System.ArgumentOutOfRangeException(nameof(dir)),
        };
    }

    /// <summary>方向をグリッド座標のオフセットに変換する</summary>
    public static Vector2Int ToOffset(Direction dir)
    {
        return dir switch
        {
            Direction.North => Vector2Int.up,
            Direction.South => Vector2Int.down,
            Direction.East => Vector2Int.right,
            Direction.West => Vector2Int.left,
            _ => throw new System.ArgumentOutOfRangeException(nameof(dir)),
        };
    }

    /// <summary>RoomDataが指定方向に通路を持つか返す</summary>
    public static bool GetPassage(RoomData room, Direction dir)
    {
        return dir switch
        {
            Direction.North => room.HasNorth,
            Direction.South => room.HasSouth,
            Direction.East => room.HasEast,
            Direction.West => room.HasWest,
            _ => throw new System.ArgumentOutOfRangeException(nameof(dir)),
        };
    }

    /// <summary>ref引数で渡した通路フラグに方向と値を適用する</summary>
    public static void ApplyConstraint(
        Direction dir, bool value,
        ref bool? needN, ref bool? needS, ref bool? needE, ref bool? needW)
    {
        switch (dir)
        {
            case Direction.North: needN = value; break;
            case Direction.South: needS = value; break;
            case Direction.East: needE = value; break;
            case Direction.West: needW = value; break;
        }
    }

    public static Direction[] All => new[]
    {
        Direction.North, Direction.South, Direction.East, Direction.West,
    };
}