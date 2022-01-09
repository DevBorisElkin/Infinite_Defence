using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameProdInstaller : MonoInstaller
{
    [Header("Scene Components")]
    public Transform PlayerInitialPos;
    public GameObject mobileInput_Joysticks;
    public CinemachineVirtualCamera mainCamera;
    public EnemiesHolderUtil enemiesHolderUtil;
    public GameManager gameManager;

    [Header("Prefabs")]
    public InputService_Mobile InputServiceMobilePrefab;
    public InputService_Desktop InputServiceDesktopPrefab;
    public Player PlayerPrefab;
    [Space(5f)] public Bullet bulletPrefab;

    private InputService inputService;

    public override void InstallBindings()
    {
        BindGameManager();
        BindEnemiesPrefabs();
        BindBullet();
        BindInputService();
        BindPlayer();
    }

    void BindGameManager()
    {
        Container.Bind<GameManager>()
            .FromInstance(gameManager)
            .AsSingle()
            .NonLazy();

        gameManager.SpawnEnemyCommand += SpawnEnemy;
    }

    void BindEnemiesPrefabs()
    {
        Container.Bind<EnemiesHolderUtil>()
            .FromInstance(enemiesHolderUtil)
            .AsSingle()
            .NonLazy();
    }
    void BindBullet()
    {
        Container.Bind<Bullet>()
            .FromInstance(bulletPrefab)
            .AsSingle()
            .NonLazy();
    }
    void BindInputService()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        inputService = Container
            .InstantiatePrefabForComponent<InputService_Desktop>(InputServiceDesktopPrefab, Vector3.zero, Quaternion.identity, null);
        mobileInput_Joysticks.SetActive(false);
#elif UNITY_ANDROID || Unity_iOS
        inputService = Container
            .InstantiatePrefabForComponent<InputService_Mobile>(InputServiceMobilePrefab, Vector3.zero, Quaternion.identity, null);
        mobileInput_Joysticks.SetActive(true);
#endif

        Container.Bind<InputService>()
        .FromInstance(inputService)
        .AsSingle()
        .NonLazy();
    }
    void BindPlayer()
    {
        Player player = Container
            .InstantiatePrefabForComponent<Player>(PlayerPrefab, PlayerInitialPos.position, Quaternion.identity, null);

        Container.Bind<Player>()
        .FromInstance(player)
        .AsSingle()
        .NonLazy();

        mainCamera.Follow = player.transform;
        mainCamera.LookAt = player.transform;

        InputService_Desktop desktop = inputService as InputService_Desktop;
        if (desktop != null) desktop.InjectPlayer(player);
    }

    public Entity SpawnEnemy(Entity selectedPrefab, Vector2 spawnPos)
    {
        var enemy = Container
            .InstantiatePrefabForComponent<Entity>(selectedPrefab, spawnPos, Quaternion.identity, null);
        return enemy;
    }

    private void OnDestroy()
    {
        gameManager.SpawnEnemyCommand -= SpawnEnemy;
    }
}
