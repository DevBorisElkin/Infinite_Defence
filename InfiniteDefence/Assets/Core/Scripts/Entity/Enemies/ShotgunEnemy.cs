using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShotgunEnemy : Enemy
{
    [SerializeField] protected float angleBetweenBullets = 6f;

    public override void MakeShot()
    {
        for (int i = -1; i < 2; i++)
        {
            var bullet = Instantiate<Bullet>(bulletPrefab, shootingPoint.position, shootingPoint.rotation * Quaternion.Euler(0, 0, angleBetweenBullets * i));
            bullet.SetUpBullet(this, gameObject.layer);
        }
    }
}
