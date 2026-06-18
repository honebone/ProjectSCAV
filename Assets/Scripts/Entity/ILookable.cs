using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視界の概念があるエンティティが実装するインターフェース
/// 主にpresenterから渡されたEntityScannerを使うために必要
/// </summary>
public interface ILookable
{
    //自身からLookAtの方向へFOVAngle度、SightRangeの扇状が見える範囲
    Vector2 LookAt { get; }//見ている場所　プレイヤーの場合は常にマウスカーソルの位置
    void Look(Vector2 lookAt, float angle, float range);
}
