using CallSOS.Player.Interaction;
using CallSOS.Player.Interaction.Equipment;
using CallSOS.Utilities;
using Michsky.UI.Heat;
using TMPro;
using UnityEngine;
using static CallSOS.Player.Interaction.InventoryController;

namespace CallSOS.Player.UI
{
    public class PlayerHudManager : MonoBehaviour
    {
        [SerializeField] private ObjectInteractController interactController;
        [SerializeField] private ProgressBar staminaBar;
        [SerializeField] private string interactKeyText;
        [SerializeField] internal GameObject invPanelBackground;
        [SerializeField] internal Transform UIInvObjectHolder;

        [SerializeField] internal NetworkedTopDownPlayerController topDownPlayerController;
        internal InventoryController inventoryController;
        public InventorySlotInfo[] hotbarSlots = new InventorySlotInfo[0];
   
        [Header("Cursor Text")]
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private RectTransform interactTextParent;
        [SerializeField] private LocalizedObject interactTextLocalize;

        [Header("Equipment Ability Text")]
        [SerializeField] private TextMeshProUGUI equipmentActionText;
        [SerializeField] private TextMeshProUGUI equipmentAltActionText;

        [SerializeField] internal EquipmentItem objectInHand;

        private float startingStaminaValue = 100f;
        private float depleatedStaminaValue = 0f;

        internal void Initialize()
        {
            staminaBar.currentValue = startingStaminaValue;
            interactText.gameObject.SetActive(false);
            equipmentActionText.gameObject.SetActive(false);
            equipmentAltActionText.gameObject.SetActive(false);

            interactController.OnInteractTextEvent += InteractController_OnInteractTextEvent;
            inventoryController.OnGetEquipment += InventoryController_OnGetEquipment;
        }

        private void Update()
        {
            UpdateCursorTextPosition();
        }

        private void UpdateCursorTextPosition()
        {
            Vector3 mousePosition = Input.mousePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                interactTextParent.parent as RectTransform,
                mousePosition,
                null,
                out Vector2 localPoint
            );

            interactTextParent.localPosition = localPoint;
        }

        private void OnDestroy()
        {
            interactController.OnInteractTextEvent -= InteractController_OnInteractTextEvent;
            inventoryController.OnGetEquipment -= InventoryController_OnGetEquipment;
        }

        private void InteractController_OnInteractTextEvent(object sender, ObjectInteractController.ChangeTextEvent e)
        {
            if (e.LocalizedKey == null)
            {
                interactText.gameObject.SetActive(false);
                return;
            }

            interactTextLocalize.localizationKey = e.LocalizedKey;
            interactText.gameObject.SetActive(true);
        }

        private void InventoryController_OnGetEquipment(object sender, InventoryController.GetEquipmentEventArgs e)
        {
            objectInHand = e.EquipmentItem;
            if (objectInHand == null)
            {
                equipmentActionText.gameObject.SetActive(false);
                equipmentAltActionText.gameObject.SetActive(false);
            }
            else
            {
                equipmentActionText.text = objectInHand.mainActionText;
                equipmentActionText.gameObject.SetActive(true);

                equipmentAltActionText.text = objectInHand.altActionText;
                equipmentAltActionText.gameObject.SetActive(true);
                
            }
        }

        private void FixedUpdate()
        {
            float sprintTimer = topDownPlayerController._timeSinceSprintStarted / topDownPlayerController.MaxSprintDuration;
            float remainingSprint = Mathf.Lerp(startingStaminaValue, depleatedStaminaValue, sprintTimer);

            staminaBar.SetValue(remainingSprint);

        }
    }
}