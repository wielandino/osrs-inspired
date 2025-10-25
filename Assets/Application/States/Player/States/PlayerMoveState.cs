using System;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private Vector3 _targetPosition;
    private PlayerStateManager _player;

    public void SetTargetPosition(Vector3 position)
    {
        // Einfach die Zielposition setzen, wird beim Enter verwendet
        _targetPosition = position;
    }

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entered PlayerMoveState");
        _player = player;

        // Null-Check für PlayerMovementController
        if (player.PlayerMovementController == null)
        {
            Debug.LogError("PlayerMovementController is null! Make sure it's attached to the GameObject.");
            return;
        }

        // Event Listener registrieren
        player.PlayerMovementController.OnMovementCompleted += OnMovementCompleted;
        player.PlayerMovementController.OnMovementCancelled += OnMovementCancelled;

        // Bewegung starten
        player.PlayerMovementController.StartMovement(_targetPosition);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Nur Input handling, Bewegung läuft in MovementController
        //player.InputHandler.HandleMouseClick();
    }

    public override void ExitState(PlayerStateManager player)
    {
        Debug.Log("Exit PlayerMoveState");

        // Events abmelden
        if (player.PlayerMovementController != null)
        {
            player.PlayerMovementController.OnMovementCompleted -= OnMovementCompleted;
            player.PlayerMovementController.OnMovementCancelled -= OnMovementCancelled;
        }
    }

    private void OnMovementCompleted()
    {
        _player.SwitchState(_player.IdleState);
    }

    private void OnMovementCancelled()
    {
        _player.SwitchState(_player.IdleState);
    }
}
