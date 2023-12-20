using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BootstrapNetworkManager : NetworkBehaviour
{
    private static BootstrapNetworkManager instance;

    private void Awake() => instance = this;

    public static event EventHandler OnGameStarted;

    public static void ChangeNetworkSceneMain(string sceneName, string[] scenesToClose)
    {
        instance.CloseScenesServer(scenesToClose);

        SceneLoadData sld = new SceneLoadData(sceneName);
        NetworkConnection[] conns = instance.ServerManager.Clients.Values.ToArray();
        instance.SceneManager.LoadConnectionScenes(conns, sld);
        instance.EnterGameSceneServer();
        OnGameStarted?.Invoke(instance, EventArgs.Empty);

    }

    [ServerRpc(RequireOwnership = false)]
    void EnterGameSceneServer()
    {
        EnterGameSceneObserver();
    }

    [ObserversRpc]
    void EnterGameSceneObserver()
    {

    }

    [ServerRpc(RequireOwnership = false)]
    void CloseScenesServer(string[] scenesToClose)
    {
        CloseScenesObserver(scenesToClose);
    }

    [ObserversRpc]
    void CloseScenesObserver(string[] scenesToClose)
    {
        foreach (var sceneName in scenesToClose)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
