using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService : MonoBehaviour, IInput
{
    public Action<Vector2> TargetMovement;
    public Action<Vector2> TargetRotation;

    public Action PlayerTriesToShoot;

    public virtual void Update() => ManageInput();

    public virtual void ManageInput()
    {
        throw new NotImplementedException();
    }
}
