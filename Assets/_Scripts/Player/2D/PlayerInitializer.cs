using CallSOS.Player.Interaction;
using CallSOS.Player.SFX;
using CallSOS.Player.UI;
using CallSOS.Utilities;
using Michsky.UI.Heat;
using FishNet.Object;
using UnityEngine;
using Cinemachine;

namespace CallSOS.Player
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInitializer : NetworkBehaviour
    {
        [SerializeField] private NetworkedTopDownPlayerController playerController;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private CursorCameraTarget cameraTarget;
        [SerializeField] private GameObject PauseMenuPrefab;
        [SerializeField] private GameObject PlayerMesh;
        [SerializeField] private GameObject playerExtrasPrefab;

        private Camera playerCamera;
        private CinemachineVirtualCamera virtualCamera;

        private PlayerExtraSpawner extraSpawner;
        private ObjectInteractController interactController;
        private PlayerSoundManager soundManager;
        private PlayerHudManager hudManager;
        private GameInputPlayer playerInput;

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

            GameObject playerExtrasGameObject = Instantiate(playerExtrasPrefab);
            extraSpawner = playerExtrasGameObject.GetComponent<PlayerExtraSpawner>();

            if (extraSpawner != null)
            {
                interactController = extraSpawner.interactController;
                soundManager = extraSpawner.soundManager;
                hudManager = extraSpawner.hudManager;
                playerInput = extraSpawner.playerInput;
                playerCamera = extraSpawner.playerCamera;
                virtualCamera = extraSpawner.virtualCamera;
            }

            Destroy(extraSpawner);

            cameraTarget.playerCamera = playerCamera;
            virtualCamera.Follow = cameraTarget.transform;

            playerInput.Initialize();

            GameObject pauseMenu = Instantiate(PauseMenuPrefab);
            playerController.SubscribedEvents(pauseMenu.GetComponentInChildren<PauseMenuManager>());

            if (interactController != null) interactController.Initialize();

            inventoryController.interactController = interactController;
            inventoryController.enabled = true;
            if (inventoryController != null) inventoryController.Initialize();

            if (soundManager != null) soundManager.Init();

            if (audioListener != null) audioListener.enabled = true;

            if (hudManager != null)
            {
                inventoryController.hotbarSlots = hudManager.hotbarSlots;
                hudManager.topDownPlayerController = playerController;
                hudManager.inventoryController = inventoryController;
                hudManager.enabled = true;
                hudManager.Initialize();
            }

            Destroy(this);
        }

    }
}