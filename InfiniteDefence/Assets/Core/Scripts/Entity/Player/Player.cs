using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player : Entity
{
    private Vector2 posJoystickInput;
    private Vector2 rotJoystickInput;

    private InputService inputService;
    private Bullet bulletPrefab;

    [Inject]
    private void Construct(InputService inputService, Bullet bulletPrefab)
    {
        this.inputService = inputService;
        this.bulletPrefab = bulletPrefab;

        inputService.TargetMovement += TargetMovementReceived;
        inputService.TargetRotation += TargetRotationReceived;
        inputService.PlayerTriesToShoot += TryToShoot;

        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        inputService.TargetMovement -= TargetMovementReceived;
        inputService.TargetRotation -= TargetRotationReceived;
        inputService.PlayerTriesToShoot -= TryToShoot;
    }

    private void TargetMovementReceived(Vector2 targetMovement) => posJoystickInput = targetMovement;
    private void TargetRotationReceived(Vector2 targetRotation) => rotJoystickInput = targetRotation;

    public override void MakeShot()
    {
        var bullet = Instantiate<Bullet>(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

        bullet.SetUpBullet();
    }

    public override void PerformMovement()
    {
        ManageMovement();
        ManageMaxForce();
    }

    public override void PerformRotation(out Vector2 angleDist)
    {
        angleDist = Vector2.zero;
        ManageRotation();
    }

    void ManageMovement()
    {
        if (posJoystickInput == Vector2.zero) return;
        Vector2 translation = new Vector2(posJoystickInput.x, posJoystickInput.y).normalized * _movementSpeed * Time.deltaTime;
        rb.AddForce(translation, movementForceMode);
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
}
