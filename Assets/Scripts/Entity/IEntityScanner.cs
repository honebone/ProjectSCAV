using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視界の範囲をもとに、見えているエンティティを返す
/// </summary>
public interface IEntityScanner
{
    IReadOnlyList<EntityModel> Scan(Vector2 toward, float fovAngle, float range,bool ignoreWall);
}
