using FishNet.Component.Spawning;
using FishNet.Object;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHolder : MonoBehaviour
{
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private KinematicCharacterMotor playerMotor;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        playerSpawner.OnSpawned += PlayerSpawner_GetPlayer;
        MainMenuManager.OnGameLaunched += Instance_OnGameLaunched;

    }

    private void Instance_OnGameLaunched(object sender, System.EventArgs e)
    {
        playerController.enabled = true;
        playerMotor.enabled = true;
        playerController.Init();
    }

    void PlayerSpawner_GetPlayer(NetworkObject playerNetworkObject)
    {
        playerObject = playerNetworkObject.gameObject;
        playerController = playerObject.GetComponentInChildren<PlayerController>();
        playerMotor = playerObject.GetComponentInChildren<KinematicCharacterMotor>();
    }
}
