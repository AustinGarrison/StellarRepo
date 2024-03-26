using CallSOS.Player.SFX;
using CallSOS.Player.UI;
using Cinemachine;
using UnityEngine;

namespace CallSOS.Utilities
{
    public class PlayerExtraSpawner : MonoBehaviour
    {
        [SerializeField] internal ObjectInteractController interactController;
        [SerializeField] internal CinemachineVirtualCamera virtualCamera;
        [SerializeField] internal PlayerSoundManager soundManager;
        [SerializeField] internal PlayerHudManager hudManager;
        [SerializeField] internal GameInputPlayer playerInput;
        [SerializeField] internal CameraFollowTarget cameraFollow;
        [SerializeField] internal Camera playerCamera;
    }
}