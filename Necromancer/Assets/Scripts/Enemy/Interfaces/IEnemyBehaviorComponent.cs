using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehaviorComponent
{
    void Initialize(Enemy enemy, Transform playerTransform);
    void OnUpdate();
    void OnEnter();
    void OnExit();
}
