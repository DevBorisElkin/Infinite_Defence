using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : Entity
{
    public Player player;
    GameManager gameManager;
    protected Vector2 randomPositionAroundPlayer = new Vector2(2f, 7f);
    [SerializeField] protected float minAngleToShoot = 5f;
    [SerializeField] protected float minDistToShoot = 15f;

    private Vector2 targetMovement;

    [Inject]
    public void Construct(Player player, GameManager gameManager)
    {
        Debug.Log("Construct for Enemy " + name);
        this.player = player;
        this.gameManager = gameManager;

        GetRandomMovementPosition();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void PerformActions()
    {
        PerformMovement();
        PerformRotation(out Vector2 angleDist);
        CheckConditionsAndTryToShoot(angleDist);
    }

    public override void PerformMovement()
    {
        if (Vector2.Distance(transform.position, targetMovement) > 1f)
        {
            Vector2 force = (targetMovement - new Vector2(transform.position.x, transform.position.y)).normalized * Time.fixedDeltaTime * _movementSpeed;

            rb.AddForce(force);
        }
        else GetRandomMovementPosition();

        ManageMaxForce();
    }

    void ManageMaxForce()
    {
        if (rb.velocity.magnitude > _maxSpeed)
            rb.velocity = rb.velocity.normalized * _maxSpeed;
    }

    public override void PerformRotation(out Vector2 angleDist)
    {
        Vector2 targetDir = player.transform.position - transform.position;
        float zAxis = Mathf.Atan2(targetDir.x, -targetDir.y) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, zAxis);

        //transform.rotation = targetRot;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);

        angleDist = new Vector2(Vector3.Angle(targetDir, -transform.up), Vector2.Distance(transform.position, player.transform.position));
    }

    void CheckConditionsAndTryToShoot(Vector2 angleDist)
    {
        Debug.Log($"Angle to the target: " + angleDist);

        if (angleDist.x <= minAngleToShoot && angleDist.y <= minDistToShoot)
            TryToShoot();
    }

    void GetRandomMovementPosition()
    {
        Vector2 generatedPos = new Vector2(player.transform.position.x, player.transform.position.y)
            + (Random.insideUnitCircle * UnityEngine.Random.Range(randomPositionAroundPlayer.x, randomPositionAroundPlayer.y));

        generatedPos = new Vector2(Mathf.Clamp(generatedPos.x, -gameManager.mapBorders.x, gameManager.mapBorders.x),
            Mathf.Clamp(generatedPos.y, -gameManager.mapBorders.y, gameManager.mapBorders.y));

        targetMovement = generatedPos;
    }

    private void OnDrawGizmos()
    {
        if (targetMovement != null)
            Gizmos.DrawCube(targetMovement, Vector3.one);
    }
}
