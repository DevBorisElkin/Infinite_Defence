using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameProdInstaller : MonoInstaller
{
    public Transform PlayerInitialPos;
    public PlayerController PlayerPrefab;
    public InputService InputServicePrefab;
    public CinemachineVirtualCamera mainCamera;

    public override void InstallBindings()
    {
        Debug.Log("InstallBindings() for LocationInstaller");

        BindInputService();
        BindPlayer();
    }

    void BindInputService()
    {
        var inputService = Container
            .InstantiatePrefabForComponent<InputService>(InputServicePrefab, Vector3.zero, Quaternion.identity, null);

        Container.Bind<IInput>()
        .To<InputService>()
        .FromInstance(inputService)
        .AsSingle()
        .NonLazy();
    }

    void BindPlayer()
    {
        var heroController = Container
            .InstantiatePrefabForComponent<PlayerController>(PlayerPrefab, PlayerInitialPos.position, Quaternion.identity, null);

        mainCamera.Follow = heroController.transform;
        mainCamera.LookAt = heroController.transform;
    }
}
