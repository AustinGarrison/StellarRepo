using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnEnable : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Start()
    {

        MainMenuManager.OnGameLaunched += MainMenuManager_OnJumpAction;
    }

    private void MainMenuManager_OnJumpAction(object sender, System.EventArgs e)
    {
        Debug.Log("GameStarted");
    }
}
