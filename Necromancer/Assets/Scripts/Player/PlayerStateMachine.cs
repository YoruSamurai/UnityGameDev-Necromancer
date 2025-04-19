using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState {  get; private set; }
    public PlayerState nextState { get; private set; }

    public PlayerState lastState { get; private set; }//上一个状态

    //初始化并进入第一个状态
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }
    //切换状态 也就是离开当前状态 设置当前状态为下一个状态 进入下一个状态
    public void ChangeState(PlayerState _newState)
    {
        lastState = currentState;
        nextState = _newState;
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
