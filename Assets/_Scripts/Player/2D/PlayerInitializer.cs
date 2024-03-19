using CallSOS.Player.Interaction;
using CallSOS.Player.SFX;
using CallSOS.Player.UI;
using CallSOS.Utilities;
using Michsky.UI.Heat;
using FishNet.Object;
using UnityEngine;

namespace CallSOS.Player
{
    public class PlayerInitializer : NetworkBehaviour
    {
        [SerializeField] private NetworkedTopDownPlayerController playerController;
        [SerializeField] private ObjectInteractController interactController;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private PlayerSoundManager playerSound;
        [SerializeField] private PlayerHudManager hudManager;
        [SerializeField] private GameInputPlayer playerInput;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private GameObject PauseMenuPrefab;
        [SerializeField] private GameObject PlayerMesh;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (base.IsOwner)
            {
                Initialize();
            }
            else
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                PlayerMesh.gameObject.SetActive(true);
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