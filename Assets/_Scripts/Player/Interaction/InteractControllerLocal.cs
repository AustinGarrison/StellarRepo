using CallSOS.Player.Interaction.Equipment;
using System;
using TMPro;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class InteractControllerLocal : MonoBehaviour
    {
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private float pickupRange = 4f;
        [SerializeField] private LayerMask interactLayer;

        private Transform _cameraTransform;
        private bool isInitialized;

        // Events
        public event EventHandler<EquipmentItemEventArgs> OnEquipmentItemInteract;
        public event EventHandler<ResourceItemEventArgs> OnResourceItemInteract;

        public class EquipmentItemEventArgs : EventArgs
        {
            public EquipmentItem EquipmentItem { get; }

            public EquipmentItemEventArgs(EquipmentItem item)
            {
                EquipmentItem = item;
            }
        }

        public class ResourceItemEventArgs : EventArgs
        {
            public ResourceItem ResourceItem { get; }

            public ResourceItemEventArgs(ResourceItem item)
            {
                ResourceItem = item;
            }
        }

        private void Awake()
        {
            interactText.gameObject.SetActive(false);
        }

        internal void Init()
        {
            GameInputPlayer.Instance.OnInteractAction += GameInput_OnInteractAction;
            _cameraTransform = Camera.main.transform;
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized) return;

            if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
            {
                InteractItem item = hit.transform.gameObject.GetComponent<InteractItem>();
                Debug.Log(hit.transform.gameObject.name);
                InteractType type = item.interactType;

                switch (type)
                {
                    case InteractType.OperationItem:

                        OperationItem opItem = item.GetComponentInParent<OperationItem>();

                        if (opItem != null)
                            interactText.text = "[E] " + opItem.interactText;
                        else
                            interactText.text = "[E] " + "Interact";

                        interactText.gameObject.SetActive(true);
                        break;

                    case InteractType.HoldItem:
                        interactText.text = "[E] " + item.itemScriptable.interactText;
                        interactText.gameObject.SetActive(true);
                        break;
                    case InteractType.InventoryItem:
                        interactText.text = "[E] " + item.itemScriptable.interactText;
                        interactText.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                interactText.gameObject.SetActive(false);
            }
        }

        private void GameInput_OnInteractAction(object sender, EventArgs e)
        {
            if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
            {
                InteractType type = hit.transform.gameObject.GetComponent<InteractItem>().interactType;

                switch (type)
                {
                    case InteractType.OperationItem:
                        InteractWithOperationItem(hit);
                        break;
                    case InteractType.HoldItem:
                        InteractWithHoldItem(hit);
                        break;
                    case InteractType.InventoryItem:
                        InteractWithInventoryItem(hit);
                        break;
                    default:
                        Debug.LogError("No interaction type found in Hit");
                        break;
                }
            }
        }

        private void InteractWithOperationItem(RaycastHit hit)
        {
            //hit.transform.GetComponent<OperationItem>().InteractWith();
            IInteractItem operation = hit.transform.GetComponent<IInteractItem>();

            if (operation != null)
            {
                operation.InteractWith(this);
            }
        }

        private void InteractWithHoldItem(RaycastHit hit)
        {
            EquipmentItem equipmentItem = hit.transform.GetComponent<EquipmentItem>();

            if (equipmentItem != null)
            {
                equipmentItem.InteractWith(this);

                OnEquipmentItemInteract?.Invoke(this, new EquipmentItemEventArgs(equipmentItem));
            }
            else
            {
                Debug.LogWarning("Failed to get hold Item");
            }
        }

        private void InteractWithInventoryItem(RaycastHit hit)
        {
            //if (hit.transform.GetComponent<InventoryItem>() == null)
            //    return;
            ResourceItem inventoryItem = hit.transform.GetComponent<ResourceItem>();

            if (inventoryItem != null)
            {
                inventoryItem.InteractWith(this);
                OnResourceItemInteract?.Invoke(this, new ResourceItemEventArgs(inventoryItem));

                // Despawn Object
                Destroy(hit.transform.gameObject);
            }
        }
    }
}