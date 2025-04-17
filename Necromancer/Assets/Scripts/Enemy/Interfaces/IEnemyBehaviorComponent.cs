using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehaviorComponent
{
    void Initialize(Enemy enemy,Player player, Transform playerTransform,MonsterStats monsterStats);
    void OnUpdate();
    void OnEnter();
    void OnExit();
}
