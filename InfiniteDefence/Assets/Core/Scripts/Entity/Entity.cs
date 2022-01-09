using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Entity : MonoBehaviour
{
    public float maxHp;
    public FloatReactiveProperty HP;
    public ReactiveCommand<Entity> EntityKilled = new ReactiveCommand<Entity>();

    [Space(5f)]
    public float _movementSpeed = 5f;
    public float _rotationSpeed = 5f;
    public float _maxSpeed = 15f;
    public ForceMode2D movementForceMode;

    protected bool canShoot;
    [Space(5f)] [SerializeField] protected float shootingCooldown = 0.5f;
    [SerializeField] protected Transform shootingPoint;

    protected List<IDisposable> LifetimeDisposables;

    protected Rigidbody2D rb;

    public virtual void Awake()
    {
        LifetimeDisposables = new List<IDisposable>();
        canShoot = true;
    }

    public virtual void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }

    public virtual void FixedUpdate() => PerformActions();

    public virtual void PerformActions()
    {
        PerformMovement();
        PerformRotation();
    }
    public virtual void PerformMovement() { throw new NotImplementedException(); }
    public virtual void PerformRotation() { throw new NotImplementedException(); }
    public virtual void TakeDamage(float damage) { throw new NotImplementedException(); }

    public virtual void TryToShoot()
    {
        if (!canShoot) return;

        canShoot = false;
        MakeShot();
        Observable.Timer(TimeSpan.FromSeconds(shootingCooldown)).Subscribe(_ => { canShoot = true; }).AddTo(LifetimeDisposables);
    }

    public virtual void MakeShot()
    {
        throw new NotImplementedException();
    }
}
