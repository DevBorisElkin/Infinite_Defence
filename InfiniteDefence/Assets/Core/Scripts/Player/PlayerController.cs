using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float _movementSpeed = 5f;
    public float _rotationSpeed = 5f;
    public float _maxSpeed = 15f;
    public ForceMode2D movementForceMode;

    private Vector2 posJoystickInput;
    private Vector2 rotJoystickInput;

    private IInput inputService;

    [Inject]
    private void Construct(IInput inputService)
    {
        this.inputService = inputService;
        inputService.TargetMovement += TargetMovementReceived;
        inputService.TargetRotation += TargetRotationReceived;

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        inputService.TargetMovement -= TargetMovementReceived;
        inputService.TargetRotation -= TargetRotationReceived;
    }

    private void TargetMovementReceived(Vector2 targetMovement) => posJoystickInput = targetMovement;
    private void TargetRotationReceived(Vector2 targetRotation)
    {
        rotJoystickInput = targetRotation;
    }

    private void FixedUpdate()
    {
        ManageMovement();
        ManageRotation();
        ManageMaxForce();
    }

    void ManageMovement()
    {
        if (posJoystickInput == Vector2.zero) return;
        Vector2 translation = new Vector2(posJoystickInput.x, posJoystickInput.y).normalized * _movementSpeed * Time.deltaTime;
        rb.AddForce(translation, movementForceMode);
    }

    void ManageRotation()
    {
        Vector2 value;

        if (posJoystickInput != Vector2.zero || rotJoystickInput != Vector2.zero)
        {
            if (rotJoystickInput != Vector2.zero) value = rotJoystickInput;
            else value = posJoystickInput;

            float zAxis = Mathf.Atan2(value.x, -value.y) * Mathf.Rad2Deg;

            Quaternion targetRot = Quaternion.Euler(0, 0, zAxis);

            //Quaternion targetRot = Quaternion.Euler(transform.rotation.x, transform.rotation.z, zAxis);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

            //TryToShoot(value, targetRot);
        }
    }

    void ManageMaxForce()
    {
        if (posJoystickInput != Vector2.zero)
        {
            if (rb.velocity.magnitude > _maxSpeed)
                rb.velocity = rb.velocity.normalized * _maxSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        rb.angularVelocity = 0;
    }
}
