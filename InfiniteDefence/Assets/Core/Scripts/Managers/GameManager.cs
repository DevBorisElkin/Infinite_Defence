using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using UniRx;
using Zenject;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] private float enemyHp;
    [SerializeField] private Vector2 spawnInterval;
    [SerializeField] private int maxEnemiesOnMap;
    public Vector2 mapBorders = new Vector2(18.5f, 18.0f);

    [Header("Too Many Enemies")]
    [SerializeField] private Vector2 spawnInterval_Increased;
    [SerializeField] private int maxEnemiesOnMap_tooManyEnemies;

    [Header("Increased Enemy Health")]
    [SerializeField] private float enemyHp_increasedEnemyHealth;
    
    Challenge assignedChallenge;

    [SerializeField] private List<Entity> enemiesPrefabs;
    private List<Entity> enemies;

    private List<IDisposable> LifetimeDisposables;

    public Func<Entity, Vector2, Entity> SpawnEnemyCommand;

    [Inject]
    public void Construct(EnemiesHolderUtil enemiesHolderUtil)
    {
        this.enemiesPrefabs = enemiesHolderUtil.enemiesPrefabs;
        LifetimeDisposables = new List<IDisposable>();

        Debug.Log("Construct finished, setting up challenge");

        SetUpChallenge((Challenge)UnityEngine.Random.Range(0, 3));
        StartRound();
    }

    private void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }

    public void SetUpChallenge(Challenge challenge)
    {
        assignedChallenge = challenge;
        Debug.Log("Chosen Challenge: " + challenge);
    }

    public void StartRound()
    {
        enemies = new List<Entity>();
        StartCoroutine(EnemySpawnCoroutine());
    }

    IEnumerator EnemySpawnCoroutine()
    {
        int maxEnemies = assignedChallenge.Equals(Challenge.Too_Many_Enemies) ? maxEnemiesOnMap_tooManyEnemies : maxEnemiesOnMap;

        while (true)
        {
            if(enemies.Count < maxEnemies)
            {
                float delay = assignedChallenge.Equals(Challenge.Too_Many_Enemies) ?
                  UnityEngine.Random.Range(spawnInterval_Increased.x, spawnInterval_Increased.y)
                : UnityEngine.Random.Range(spawnInterval.x, spawnInterval.y);

                yield return new WaitForSeconds(delay);

                SpawnEnemy();
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void SpawnEnemy()
    {
        Entity selectedPrefab = assignedChallenge.Equals(Challenge.New_Enemy_Types) ?
            enemiesPrefabs[UnityEngine.Random.Range(0, enemiesPrefabs.Count)] :
            enemiesPrefabs[0];

        var enemy = SpawnEnemyCommand?.Invoke(selectedPrefab, GetRandomSpawnPosition());

        if(enemy != null)
        {
            enemies.Add(enemy);
            enemy.EntityKilled.Subscribe(_ => OnEntityDead(_)).AddTo(LifetimeDisposables);
        }
    }

    void OnEntityDead(Entity entity) => enemies.Remove(entity);

    Vector3 GetRandomSpawnPosition()
    {
        int randomSpawnPos = UnityEngine.Random.Range(0, 4);
        if (randomSpawnPos == 0) return new Vector3(-mapBorders.x, UnityEngine.Random.Range(-mapBorders.y, mapBorders.y));
        if (randomSpawnPos == 1) return new Vector3( mapBorders.x, UnityEngine.Random.Range(-mapBorders.y, mapBorders.y));
        if (randomSpawnPos == 2) return new Vector3( UnityEngine.Random.Range(-mapBorders.x, mapBorders.x), -mapBorders.y);
        if (randomSpawnPos == 3) return new Vector3( UnityEngine.Random.Range(-mapBorders.x, mapBorders.x), mapBorders.y);
        return Vector3.zero;
    }
}
