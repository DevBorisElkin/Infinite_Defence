using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService_Mobile : InputService
{
    public override void ManageInput()
    {
        HandleMovementAndRotation(out Vector2 rotationInput);
        HandleShootingInput(rotationInput);
    }

    void HandleMovementAndRotation(out Vector2 rotationInput)
    {
        Vector2 movementInput = new Vector2(
            UltimateJoystick.GetHorizontalAxis("LeftJoystick"),
            UltimateJoystick.GetVerticalAxis("LeftJoystick")
        );

        rotationInput = new Vector2(
            UltimateJoystick.GetHorizontalAxis("RightJoystick"),
            UltimateJoystick.GetVerticalAxis("RightJoystick")
        );

        TargetMovement?.Invoke(movementInput);
        TargetRotation?.Invoke(rotationInput);
    }

    void HandleShootingInput(Vector2 rotationInput)
    {
        if (rotationInput.x >= 0.5f || rotationInput.y >= 0.5f) PlayerTriesToShoot?.Invoke();
    }
}
