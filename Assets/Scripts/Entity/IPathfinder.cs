using UnityEngine;

/// <summary>
/// 経路探索機能を提供するインターフェース
/// EntityViewが実装し、ChasePlayerEnemyModelに注入する
/// </summary>
public interface IPathfinder
{
    /// <summary>
    /// fromからtoへの経路を返す
    /// toが空中にある場合は真下の足場を目標とする
    /// jumpHeight/jumpWidthで使用できるエッジを絞り込む
    /// </summary>
    NavPath FindPath(Vector2 from, Vector2 to, int jumpHeight, int jumpWidth);

    NavPathfinder NavPathfinder { get; }
}