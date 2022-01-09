using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameProdInstaller : MonoInstaller
{
    public Transform PlayerInitialPos;
    public Player PlayerPrefab;
    public InputService_Mobile InputServiceMobilePrefab;
    public InputService_Desktop InputServiceDesktopPrefab;
    public GameObject mobileInput_Joysticks;
    public CinemachineVirtualCamera mainCamera;

    [Space(5f)] public Bullet bulletPrefab;

    public override void InstallBindings()
    {
        Debug.Log("InstallBindings() for LocationInstaller");

        BindBullet();
        BindInputService();
        BindPlayer();
    }

    void BindBullet()
    {
        Container.Bind<Bullet>()
            .FromInstance(bulletPrefab)
            .NonLazy();
    }

    void BindInputService()
    {
        InputService inputService;

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
        var heroController = Container
            .InstantiatePrefabForComponent<Player>(PlayerPrefab, PlayerInitialPos.position, Quaternion.identity, null);

        mainCamera.Follow = heroController.transform;
        mainCamera.LookAt = heroController.transform;
    }
}
