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
        [SerializeField] private GameObject PauseMenuPrefab;
        [SerializeField] private GameObject PlayerMesh;
        [SerializeField] private GameObject playerExtrasPrefab;

        private CinemachineVirtualCamera virtualCamera;

        private PlayerExtraSpawner extraSpawner;
        //private CursorCameraTarget cameraTarget;
        private ObjectInteractController interactController;
        private PlayerSoundManager soundManager;
        [SerializeField] private PlayerHudManager hudManager;
        private GameInputPlayer playerInput;

        //Delete
        private CameraFollowTarget cameraFollow;

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
                virtualCamera = extraSpawner.virtualCamera;
                cameraFollow = extraSpawner.cameraFollow;
            }

            Destroy(extraSpawner);

            if (cameraFollow != null)
            {
                cameraFollow.player = playerController.transform;
            }

            if (virtualCamera != null)
                virtualCamera.Follow = cameraFollow.transform;

            if(playerInput != null)
                playerInput.Initialize();

            GameObject pauseMenu = Instantiate(PauseMenuPrefab);
            playerController.SubscribedEvents(pauseMenu.GetComponentInChildren<PauseMenuManager>());

            if (interactController != null)
                interactController.Initialize();

            if(inventoryController != null)
            {
                inventoryController.interactController = interactController;
                inventoryController.enabled = true;
            }

            if (inventoryController != null) 
                inventoryController.Initialize();

            if (soundManager != null)
                soundManager.Init();

            if (audioListener != null)
                audioListener.enabled = true;

            if (hudManager != null)
            {
                inventoryController.hotbarSlots = hudManager.hotbarSlots;
                hudManager.topDownPlayerController = playerController;
                hudManager.inventoryController = inventoryController;
                hudManager.enabled = true;
                hudManager.Initialize();

                inventoryController.invPanelBackground = hudManager.invPanelBackground;
                inventoryController.UIInvObjectHolder = hudManager.UIInvObjectHolder;
            }

            Destroy(this);
        }

    }
}