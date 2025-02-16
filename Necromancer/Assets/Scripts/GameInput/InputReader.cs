using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IUIActions
{
    private GameInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            SetGameplay();
        }
    }

    public void SetGameplay()
    {
        _gameInput.Gameplay.Enable();
        _gameInput.UI.Disable();
    }

    public void SetUI()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.UI.Enable();
    }

    public event Action<Vector2> MoveEvent;

    public event Action JumpEvent;
    public event Action JumpCancelEvent;

    public event Action DashEvent;
    public event Action RollEvent;


    public void OnNewaction(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"Phase:{context.phase},value:{context.ReadValue<Vector2>()}");
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            JumpCancelEvent?.Invoke();
        }
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        RollEvent?.Invoke();
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        DashEvent?.Invoke();
    }


}
