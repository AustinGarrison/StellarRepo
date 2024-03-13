using CallSOS.Player.Interaction;
using CallSOS.Player.SFX;
using CallSOS.Player.UI;
using CallSOS.Utilities;
using FishNet.Object;
using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player
{
    public class PlayerInitializer : NetworkBehaviour
    {
        [SerializeField] private GameObject PauseMenuPrefab;
        [SerializeField] private NetworkedTopDownPlayerController playerController;
        [SerializeField] private ObjectInteractController interactController;
        [SerializeField] private PlayerSoundManager playerSound;
        [SerializeField] private GameInputPlayer playerInput;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private PlayerHudManager hudManager;
        [SerializeField] private AudioListener audioListener;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (base.IsOwner)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            playerInput.Initialize();

            GameObject pauseMenu = Instantiate(PauseMenuPrefab);
            playerController.SubscribedEvents(pauseMenu.GetComponentInChildren<PauseMenuManager>());

            if (interactController != null) interactController.Initialize();

            inventoryController.enabled = true;
            if (inventoryController != null) inventoryController.Initialize();

            if (playerSound != null) playerSound.Init();

            if (audioListener != null) audioListener.enabled = true;

            if (hudManager != null)
            {
                hudManager.enabled = true;
                hudManager.Initialize();
            }

            Destroy(this);
        }
    }
}