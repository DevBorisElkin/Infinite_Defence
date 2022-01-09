using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService : MonoBehaviour, IInput
{
    public event Action<Vector2> TargetMovement;

    public event Action<Vector2> TargetRotation;

    void Update() => ManageJoystickInput();

    public void ManageJoystickInput()
    {
        float movementHorizontalInput = UltimateJoystick.GetHorizontalAxis("LeftJoystick");
        float movementVerticalInput = UltimateJoystick.GetVerticalAxis("LeftJoystick");
        float rotationHorizontalInput = UltimateJoystick.GetHorizontalAxis("RightJoystick");
        float rotationVerticalInput = UltimateJoystick.GetVerticalAxis("RightJoystick");

        TargetMovement?.Invoke(new Vector2(movementHorizontalInput, movementVerticalInput));
        TargetRotation?.Invoke(new Vector2(rotationHorizontalInput, rotationVerticalInput));
    }
}
