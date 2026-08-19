using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMover
{
    void Walk(float moveX);
    void Jump(Vector2 jump);
    void SetMove(Vector2 move);
    void SetMoveX(float moveX);
    void SetMoveY(float moveY);
    void AddMoveY(float moveY);

    /// <summary>Œ»İÚ’n‚µ‚Ä‚¢‚é‚©</summary>
    bool IsGrounded { get; }
    /// <summary>d—Í‰Á‘¬“x ‰ºŒü‚«‚ª³</summary>
    float Gravity { get; }
}
