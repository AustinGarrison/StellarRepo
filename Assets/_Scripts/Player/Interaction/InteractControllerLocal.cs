using CallSOS.Player.Interaction.Equipment;
using System;
using TMPro;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class InteractControllerLocal : MonoBehaviour
    {
        //[SerializeField] private InventoryController inventoryController;
        [SerializeField] private float pickupRange = 4f;
        [SerializeField] private LayerMask interactLayer;

        private Transform _cameraTransform;
        private bool isInitialized;
        public bool pickupItem = false;

        #region Events
        // Events
        public event EventHandler<EquipmentItemEventArgs> OnEquipmentItemInteract;
        public event EventHandler<ResourceItemEventArgs> OnResourceItemInteract; 
        public event EventHandler<ChangeTextEvent> OnInteractTextEvent;

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

        public class ChangeTextEvent : EventArgs
        {
            public string Message { get; }

            public ChangeTextEvent(string message)
            {
                Message = message;
            }
        }
        #endregion

        internal void Init()
        {
            GameInputPlayer.Instance.OnInteractAction += GameInput_OnInteractAction;
            _cameraTransform = Camera.main.transform;
            isInitialized = true;
        }

        private void OnDisable()
        {
            GameInputPlayer.Instance.OnInteractAction -= GameInput_OnInteractAction;
        }

        private void GameInput_OnInteractAction(object sender, EventArgs e)
        {
            pickupItem = true;
        }

        private void Update()
        {
            if (!isInitialized) return;

            FindInteractItem();
        }

        InteractItem interactItemHolder = null;

        private void FindInteractItem()
        {
            if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit rayHit, pickupRange, interactLayer))
            {
                InteractItem interactItem = rayHit.transform.gameObject.GetComponent<InteractItem>();

                if (interactItem != interactItemHolder)
                {
                    interactItemHolder = interactItem;
                    SetPickupText(interactItem);
                }

                if (pickupItem == true)
                    PickupItem(interactItem);
            }
            else
            {
                if (interactItemHolder != null)
                {
                    interactItemHolder = null;
                    SetPickupText(null);
                }
            }
        }

        private void SetPickupText(InteractItem interactItem)
        {
            if(interactItem == null)
            {
                OnInteractTextEvent?.Invoke(this, new ChangeTextEvent(null));
                return;
            }

            string itemInteractTypeText = "";
            InteractType interactType = interactItem.interactType;

            switch (interactType)
            {
                case InteractType.OperationItem:

                    OperationItem operationItem = interactItem.GetComponentInParent<OperationItem>();

                    if (operationItem != null)
                        itemInteractTypeText = operationItem.interactText;
                    else
                        itemInteractTypeText = "Interact";
                    break;

                case InteractType.HoldItem:
                    itemInteractTypeText = interactItem.itemScriptable.interactText;
                    break;

                case InteractType.InventoryItem:
                    itemInteractTypeText = interactItem.itemScriptable.interactText;
                    break;

            }

            OnInteractTextEvent?.Invoke(this, new ChangeTextEvent(itemInteractTypeText));            
        }

        private void PickupItem(InteractItem interactItem)
        {
            pickupItem = false;

            InteractType type = interactItem.interactType;

            switch (type)
            {
                case InteractType.OperationItem:
                    InteractWithOperationItem(interactItem);
                    break;
                case InteractType.HoldItem:
                    InteractWithHoldItem(interactItem);
                    break;
                case InteractType.InventoryItem:
                    InteractWithInventoryItem(interactItem);
                    break;
                default:
                    Debug.LogError("No interaction type found in Hit");
                    break;
            }
        }

        private void InteractWithOperationItem(InteractItem interactItem)
        {
            //hit.transform.GetComponent<OperationItem>().InteractWith();
            IInteractItem operation = interactItem.GetComponent<IInteractItem>();

            if (operation != null)
            {
                operation.InteractWith(this);
            }
        }

        private void InteractWithHoldItem(InteractItem interactItem)
        {
            EquipmentItem equipmentItem = interactItem.GetComponent<EquipmentItem>();

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

        private void InteractWithInventoryItem(InteractItem interactItem)
        {
            //if (hit.transform.GetComponent<InventoryItem>() == null)
            //    return;
            ResourceItem inventoryItem = interactItem.GetComponent<ResourceItem>();

            if (inventoryItem != null)
            {
                inventoryItem.InteractWith(this);
                OnResourceItemInteract?.Invoke(this, new ResourceItemEventArgs(inventoryItem));

                // Despawn Object
                Destroy(interactItem.transform.gameObject);
            }
        }
    }
}