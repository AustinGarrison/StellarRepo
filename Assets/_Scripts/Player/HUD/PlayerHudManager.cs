using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.UI
{
    public class PlayerHudManager : MonoBehaviour
    {
        [SerializeField] private PlayerControllerLocal playerController;
        [SerializeField] private ProgressBar staminaBar;

        private void Start()
        {
            staminaBar.currentValue = 100f;
        }

        private void FixedUpdate()
        {
            float sprintTimer = playerController._timeSinceSprintStarted / playerController.MaxSprintDuration;
            float remainingSprint = Mathf.Lerp(100f, 0f, sprintTimer);

            staminaBar.SetValue(remainingSprint);
        }
    }
}