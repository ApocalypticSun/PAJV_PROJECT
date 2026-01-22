using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

 public class AdvancedMovementControls : MovementControls
    {
        public event System.Action SlamButtonPressed;
        public event System.Action DashButtonPressed;

        private NewControls _gameControls;

        private void EnsureControls()
{
    if (_gameControls == null)
        _gameControls = new NewControls();
}

public override void Initialize()
{
    EnsureControls();
    BindInputs();
}

private void BindInputs()
{
    // Move
    _gameControls.PlayerControls.Move.performed += ctx => MoveDirection = ctx.ReadValue<Vector2>();
    _gameControls.PlayerControls.Move.canceled  += ctx => MoveDirection = Vector2.zero;

    // Sprint
    _gameControls.PlayerControls.Sprint.performed += _ => OnStartSprint();
    _gameControls.PlayerControls.Sprint.canceled  += _ => OnStopSprint();

    // Jump
    _gameControls.PlayerControls.Jump.performed += _ => OnJump();

    // Slam / Dash
    _gameControls.PlayerControls.Slam.performed += _ => SlamButtonPressed?.Invoke();
    _gameControls.PlayerControls.Dash.performed += _ => DashButtonPressed?.Invoke();
}

private void OnEnable()
{
    EnsureControls();
    _gameControls.Enable();
}

private void OnDisable()
{
    _gameControls?.Disable();
}

    }

