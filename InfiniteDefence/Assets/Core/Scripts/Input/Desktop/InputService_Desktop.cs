using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputService_Desktop : InputService
{
    Vector2 screenResolutions;

    private void Awake() => screenResolutions = new Vector2(Screen.width, Screen.height);

    public override void ManageInput() => HandlePlayerFullControls();

    void HandlePlayerFullControls()
    {
        Vector2 mousePos = Input.mousePosition;

        float movementHorizontalInput = 0;
        float movementVerticalInput = 0;
        float rotationHorizontalInput = Remap(mousePos.x, 0, screenResolutions.x, -1f, 1f);
        float rotationVerticalInput = Remap(mousePos.y, 0, screenResolutions.y, -1f, 1f);

        if (Input.GetKey(KeyCode.A)) movementHorizontalInput -= 1;
        if (Input.GetKey(KeyCode.D)) movementHorizontalInput += 1;
        if (Input.GetKey(KeyCode.W)) movementVerticalInput += 1;
        if (Input.GetKey(KeyCode.S)) movementVerticalInput -= 1;

        TargetMovement?.Invoke(new Vector2(movementHorizontalInput, movementVerticalInput));
        TargetRotation?.Invoke(new Vector2(rotationHorizontalInput, rotationVerticalInput));

        if (Input.GetMouseButtonDown(0)) PlayerTriesToShoot?.Invoke();
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
