using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : Entity
{
    public Player player;
    protected Vector2 randomPositionAroundPlayer = new Vector2(2f, 7f);
    [SerializeField] protected float minAngleToShoot = 5f;
    [SerializeField] protected float minDistToShoot = 15f;

    private Vector2 targetMovement;

    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
        GetRandomMovementPosition();
    }

    public void AddMaxHp(float additionalHp)
    {
        if(additionalHp > 0)
        {
            maxHp += additionalHp;
            HP.Value = maxHp;
        }
    }

    public override void PerformActions()
    {
        if (!active_gameplay) return;
        PerformMovement();
        PerformRotation(out Vector2 angleDist);
        CheckConditionsAndTryToShoot(angleDist);
    }

    public override void PerformMovement()
    {
        ManageMovement();
        ManageMaxForce();
    }

    void ManageMovement()
    {
        if (Vector2.Distance(transform.position, targetMovement) > 1f)
        {
            Vector2 force = (targetMovement - new Vector2(transform.position.x, transform.position.y)).normalized * Time.fixedDeltaTime * _movementSpeed;

            rb.AddForce(force);
        }
        else GetRandomMovementPosition();
    }
    
    public override void PerformRotation(out Vector2 angleDist)
    {
        Vector2 targetDir = player.transform.position - transform.position;
        float zAxis = Mathf.Atan2(targetDir.x, -targetDir.y) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, zAxis);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);

        angleDist = new Vector2(Vector3.Angle(targetDir, -transform.up), Vector2.Distance(transform.position, player.transform.position));
    }

    void CheckConditionsAndTryToShoot(Vector2 angleDist)
    {
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

    // Uncomment to see position's generation
    //private void OnDrawGizmos()
    //{
    //    if (targetMovement != null)
    //        Gizmos.DrawCube(targetMovement, Vector3.one);
    //}
}
