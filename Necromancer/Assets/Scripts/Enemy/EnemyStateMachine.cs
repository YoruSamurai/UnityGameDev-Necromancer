using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState currentState { get; private set; }

    //初始化并进入第一个状态
    public void Initialize(EnemyState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }
    //切换状态 也就是离开当前状态 设置当前状态为下一个状态 进入下一个状态
    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
