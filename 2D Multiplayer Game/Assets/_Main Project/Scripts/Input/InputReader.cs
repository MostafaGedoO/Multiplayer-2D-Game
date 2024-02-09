using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    public event Action<bool> OnFireEvent;
    public event Action<Vector2> OnMoveEvent;

    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    private void OnDisable()
    {
        if(controls != null)
        {
            controls.Player.Disable();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            OnFireEvent?.Invoke(true);
        }
        else if(context.canceled)
        {
            OnFireEvent?.Invoke(false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
}
