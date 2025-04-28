using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool DashWasPressed;
    public static bool AttackWasPressed;
    public static bool PowerWasPressed;
    public static bool PauseWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAcion;
    private InputAction _attackAction;
    private InputAction _powerAction;
    private InputAction _pauseAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _dashAcion = PlayerInput.actions["Dash"];
        _attackAction = PlayerInput.actions["Kick"];
        _powerAction = PlayerInput.actions["Power"];
        _pauseAction = PlayerInput.actions["Pause"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        
        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        
        RunIsHeld = _runAction.IsPressed();

        DashWasPressed = _dashAcion.WasPressedThisFrame();

        AttackWasPressed = _attackAction.WasPressedThisFrame();

        PowerWasPressed = _powerAction.WasPressedThisFrame(); 
        
        PauseWasPressed = _pauseAction.WasPressedThisFrame();
    }
}
