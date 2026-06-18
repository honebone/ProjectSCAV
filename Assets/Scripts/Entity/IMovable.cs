using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動可能なエンティティが実装するインターフェース
/// </summary>
public interface IMovable
{
    /// <summary>移動命令を受け取る</summary>
    void Walk(float walk);

    /// <summary>ジャンプ命令</summary>
    void Jump(Vector2 jump);

    bool IsJumping { get; }
}
