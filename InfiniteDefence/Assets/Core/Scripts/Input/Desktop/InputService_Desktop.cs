using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputService_Desktop : InputService
{
    private Player player;
    public void InjectPlayer(Player player) => this.player = player;

    public override void ManageInput() => HandlePlayerFullControls();

    void HandlePlayerFullControls()
    {
        Vector2 mousePos = Input.mousePosition;

        float movementHorizontalInput = 0;
        float movementVerticalInput = 0;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(player.transform.position);

        float rotationHorizontalInput = mousePos.x - objectPos.x;
        float rotationVerticalInput = mousePos.y - objectPos.y;

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
