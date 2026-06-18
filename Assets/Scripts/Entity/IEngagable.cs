using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交戦状態に入ることができるエンティティが実装するインターフェース
/// </summary>
public interface IEngagable
{
    EntityModel Target { get; }
    bool Engaged { get; }

    void Engage(EntityModel target);
    void Disengage();
}
