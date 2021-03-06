using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using UniRx;
using Zenject;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] private Vector2 spawnInterval;
    [SerializeField] private int maxEnemiesOnMap;
    public Vector2 mapBorders = new Vector2(18.5f, 18.0f);

    [Header("Too Many Enemies")]
    [SerializeField] private Vector2 spawnInterval_Increased;
    [SerializeField] private int maxEnemiesOnMap_tooManyEnemies;

    [Header("Increased Enemy Health")]
    [SerializeField] private float enemyHp_additionalEnemyHealth = 15;
    
    Challenge assignedChallenge;
    public ReactiveProperty<GameState> AssignedGameState = new ReactiveProperty<GameState>();

    [SerializeField] private List<Entity> enemiesPrefabs;
    private List<Entity> enemies;

    private List<IDisposable> LifetimeDisposables;

    UI_Manager ui_manager;

    public Func<Entity, Vector2, Entity> SpawnEnemyCommand;

    [Inject]
    public void Construct(EnemiesHolderUtil enemiesHolderUtil, UI_Manager ui_manager)
    {
        LifetimeDisposables = new List<IDisposable>();

        this.enemiesPrefabs = enemiesHolderUtil.enemiesPrefabs;
        this.ui_manager = ui_manager;

        AssignedGameState.Value = GameState.ChallengeChoice;
        ui_manager.SetUiState(AssignedGameState.Value);

        ui_manager.ChallengeWasChosen.Subscribe(_ => 
        {
            SetUpChallenge(_);
        }).AddTo(LifetimeDisposables);

        ui_manager.PlayButtonPressed.Subscribe(_ => 
        {
            StartRound();
        }).AddTo(LifetimeDisposables);

        ui_manager.RestartButtonPressed.Subscribe(_ =>
        {
            RestartGame();
        }).AddTo(LifetimeDisposables);
    }
    public void InjectPlayer(Player player)
    {
        player.HP.Subscribe(_ =>
        {
            if (player.HP.Value <= 0 && AssignedGameState.Value != GameState.End)
            {
                // Game Over basically
                AssignedGameState.Value = GameState.End;
                ui_manager.SetUiState(AssignedGameState.Value);
            }
        }).AddTo(LifetimeDisposables);
    }
    void SetUpChallenge(Challenge challenge)
    {
        assignedChallenge = challenge;
    }

    void StartRound()
    {
        AssignedGameState.Value = GameState.Game;
        ui_manager.SetUiState(AssignedGameState.Value);

        enemies = new List<Entity>();
        StartCoroutine(EnemySpawnCoroutine());
    }

    // Simple and quick restart, could have deleted prefab of level and instantiated again
    void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    IEnumerator EnemySpawnCoroutine()
    {
        int maxEnemies = assignedChallenge.Equals(Challenge.Too_Many_Enemies) ? maxEnemiesOnMap_tooManyEnemies : maxEnemiesOnMap;

        while (AssignedGameState.Value.Equals(GameState.Game))
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
        if (!AssignedGameState.Value.Equals(GameState.Game)) return;

        Entity selectedPrefab = assignedChallenge.Equals(Challenge.New_Enemy_Types) ?
            enemiesPrefabs[UnityEngine.Random.Range(0, enemiesPrefabs.Count)] :
            enemiesPrefabs[0];

        float additionalHp = assignedChallenge.Equals(Challenge.Increased_Enemy_Health) ? enemyHp_additionalEnemyHealth : 0f;

        var enemy = SpawnEnemyCommand?.Invoke(selectedPrefab, GetRandomSpawnPosition());

        if(enemy != null)
        {
            Enemy _enemy = enemy as Enemy;
            if(_enemy != null)
                _enemy.AddMaxHp(additionalHp);
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

    private void OnDestroy()
    {
        foreach (var a in LifetimeDisposables)
            a.Dispose();
    }
}
