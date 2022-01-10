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

    protected bool canShoot;
    public float attackDamage = 10f;
    [Space(5f)] [SerializeField] protected float shootingCooldown = 0.5f;
    [SerializeField] protected Transform shootingPoint;

    protected List<IDisposable> LifetimeDisposables;

    protected GameManager gameManager;

    [SerializeField] protected Rigidbody2D rb;

    [Header("HealthBarRelated")]
    [SerializeField] protected GameObject healthBarHolder;
    [SerializeField] protected Image healthBar;

    protected bool active_gameplay;
    protected Bullet bulletPrefab;

    [Inject]
    protected void Construct_General(GameManager gameManager, Bullet bulletPrefab)
    {
        this.bulletPrefab = bulletPrefab;
        LifetimeDisposables = new List<IDisposable>();
        this.gameManager = gameManager;

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
