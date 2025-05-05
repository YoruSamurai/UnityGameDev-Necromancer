using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentEnemyState {  get; set; }
    public bool isLocked = false;
    public void Initialize(EnemyState _enemyState)
    {
        currentEnemyState = _enemyState;
        currentEnemyState.EnterState();
    }

    public void ChangeState(EnemyState _enemyState)
    {
        if (isLocked)
            return;
        currentEnemyState.ExitState();
        currentEnemyState = _enemyState;
        currentEnemyState.EnterState();
    }

    public void LockStateMachine()
    {
        isLocked = true;
    }
}
