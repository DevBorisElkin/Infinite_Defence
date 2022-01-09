using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Bullet : MonoBehaviour
{
    private int playerLayer;
    private int enemyLayer;

    private bool activated;

    private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSpeed = 15;
    [SerializeField] private ForceMode2D forceMode;

    [Space(5f)] [SerializeField] private float destroyAfter = 10f;

    List<IDisposable> LifetimeDisposables;

    private float zRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetUpBullet()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        activated = true;
        LifetimeDisposables = new List<IDisposable>();

        Observable.Timer(TimeSpan.FromSeconds(destroyAfter)).Subscribe(_ => { Destroy(gameObject); }).AddTo(LifetimeDisposables);
    }

    private void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void FixedUpdate()
    {
        if (!activated) return;

        ManageMovement();
        ManageMaxForce();
    }

    void ManageMovement() => rb.AddForce(transform.up * speed * Time.fixedDeltaTime, forceMode);

    void ManageMaxForce()
    {
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }
}
