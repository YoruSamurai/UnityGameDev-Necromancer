using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMoveable
{
    Rigidbody2D rb { get; set; }
    bool IsFacingRight {  get; set; }

    void SetVelocity(Vector2 velocity);
    void CheckFacing(Vector2 velocity);


}
