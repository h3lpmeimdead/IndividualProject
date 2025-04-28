using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyInputManager : MonoBehaviour
{
    public static PlayerInput Player2Input;

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
        Player2Input = GetComponent<PlayerInput>();
        _moveAction = Player2Input.actions["Move"];
        _jumpAction = Player2Input.actions["Jump"];
        _runAction = Player2Input.actions["Run"];
        _dashAcion = Player2Input.actions["Dash"];
        _attackAction = Player2Input.actions["Kick"];
        _powerAction = Player2Input.actions["Power"];
        _pauseAction = Player2Input.actions["Pause"];
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
