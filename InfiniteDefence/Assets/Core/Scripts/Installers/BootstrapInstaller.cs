using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("InstallBindings() for BootstrapInstaller");
        SceneManager.LoadScene(1);
    }
}