using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SimpleEnemy : Enemy
{
    public Bullet bulletPrefab;
    [Inject]
    private void Construct(Bullet bulletPrefab)
    {
        this.bulletPrefab = bulletPrefab;
        rb = GetComponent<Rigidbody2D>();
    }
    public override void MakeShot()
    {
        var bullet = Instantiate<Bullet>(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.SetUpBullet();
    }
}
