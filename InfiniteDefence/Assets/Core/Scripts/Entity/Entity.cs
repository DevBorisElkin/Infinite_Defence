using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using static Enums;

public class Entity : MonoBehaviour
{
    protected float maxHp;
    public FloatReactiveProperty HP;
    public ReactiveCommand<Entity> EntityKilled = new ReactiveCommand<Entity>();

    [Space(5f)]
    public float _movementSpeed = 5f;
    public float _rotationSpeed = 5f;
    public float _maxSpeed = 15f;
    public ForceMode2D movementForceMode;

    [Space(5f)] public readonly float attackDamage = 10f;
    [SerializeField] protected float shootingCooldown = 0.5f;
    [SerializeField] protected Transform shootingPoint;

    protected List<IDisposable> LifetimeDisposables;
    protected GameManager gameManager;
    protected Bullet bulletPrefab;
    protected Rigidbody2D rb;
    protected bool canShoot;
    protected bool active_gameplay;

    [Header("HealthBarRelated")]
    [SerializeField] protected GameObject healthBarHolder;
    [SerializeField] protected Image healthBar;


    [Inject]
    protected void Construct_General(GameManager gameManager, Bullet bulletPrefab)
    {
        this.bulletPrefab = bulletPrefab;
        this.gameManager = gameManager;
        LifetimeDisposables = new List<IDisposable>();
        rb = GetComponent<Rigidbody2D>();

        active_gameplay = gameManager.Equals(GameState.Game) ? true : false;

        gameManager.AssignedGameState.Subscribe(_ => {
            active_gameplay = _.Equals(GameState.Game) ? true : false;
        }).AddTo(LifetimeDisposables);
    }

    public virtual void Awake()
    {
        canShoot = true;

        maxHp = HP.Value;
        ManageHealthBar();
    }

    public virtual void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }

    public virtual void FixedUpdate() => PerformActions();

    public virtual void PerformActions()
    {
        if (!active_gameplay) return;
        PerformMovement();
        PerformRotation(out Vector2 angleToTarget);
    }
    public virtual void PerformMovement() { throw new NotImplementedException(); }

    public virtual void ManageMaxForce()
    {
        if (rb.velocity.magnitude > _maxSpeed)
            rb.velocity = rb.velocity.normalized * _maxSpeed;
    }
    public virtual void PerformRotation(out Vector2 angleAndDistance) { throw new NotImplementedException(); }
    
    public virtual void TryToShoot()
    {
        if (!canShoot || !active_gameplay) return;

        canShoot = false;
        MakeShot();
        Observable.Timer(TimeSpan.FromSeconds(shootingCooldown)).Subscribe(_ => { canShoot = true; }).AddTo(LifetimeDisposables);
    }

    public virtual void MakeShot()
    {
        var bullet = Instantiate<Bullet>(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.SetUpBullet(this, gameObject.layer);
    }

    #region Health, Healthbar and Death
    public virtual void TakeDamage(float damage)
    {
        HP.Value -= damage;
        ManageHealthBar();

        if (HP.Value <= 0)
            Die();
    }
    public virtual void Die()
    {
        EntityKilled.Execute(this);
        Destroy(gameObject);
    }

    Tween healthBarTween;
    void ManageHealthBar()
    {
        if (HP.Value == maxHp)
        {
            healthBarHolder.SetActive(false);
            healthBar.fillAmount = 1f;
        }
        else
        {
            healthBarHolder.SetActive(true);
            DOTween.Kill(healthBarTween);
            healthBar.DOFillAmount(HP.Value / maxHp, 0.3f);
        }
    }
    #endregion
}
