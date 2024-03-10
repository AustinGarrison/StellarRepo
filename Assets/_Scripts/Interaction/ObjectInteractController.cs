using CallSOS.Player.Interaction;
using CallSOS.Player.Interaction.Equipment;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CallSOS.Utilities
{
    public class ObjectInteractController : MonoBehaviour
    {      
        [SerializeField] float raycastRadius = 1f;

        private bool isInitialized = false;
        private bool attemptItemPickup = false;

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
            public string LocalizedKey { get; }

            public ChangeTextEvent(string localizedKey)
            {
                LocalizedKey = localizedKey;
            }
        }

        #endregion

        public void Initialize()
        {
            GameInputPlayer.Instance.OnClickAction += Instance_OnClickAction;           
            isInitialized = true;
        }

        private void OnDestroy()
        {
            GameInputPlayer.Instance.OnClickAction -= Instance_OnClickAction;
        }

        private void Instance_OnClickAction(object sender, EventArgs e)
        {
            attemptItemPickup = true;
        }

        private void Update()
        {
            if (!isInitialized) return;
            if (InteractWithUI()) return;
            if (InteractWithComponent3D()) return;
            CursorController.Instance.SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                CursorController.Instance.SetCursor(CursorType.UI);

                return true;
            }

            return false;
        }

        private bool InteractWithComponent3D()
        {
            RaycastHit[] hits = RaycastAllSorted3D();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if(raycastable.CanHandleRaycast(this))
                    {
                        CursorController.Instance.SetCursor(raycastable.GetCursorType());

                        InteractItem interactItem = raycastable.GetInteractItem();

                        if (interactItem != null)
                        {
                            SetPickupText(interactItem);

                            if (attemptItemPickup == true)
                                InteractWithItem(interactItem);
                        }

                        return true;
                    }
                }
            }

            attemptItemPickup = false;
            SetPickupText(null);
            return false;
        }

        private void SetPickupText(InteractItem interactItem)
        {
            if (interactItem == null)
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
                        itemInteractTypeText = operationItem.localizationKey;
                    else
                        itemInteractTypeText = "Interact";
                    break;

                case InteractType.HoldItem:
                    itemInteractTypeText = interactItem.itemScriptable.localizationKey;
                    break;

                case InteractType.InventoryItem:
                    itemInteractTypeText = interactItem.itemScriptable.localizationKey;
                    break;

            }

            OnInteractTextEvent?.Invoke(this, new ChangeTextEvent(itemInteractTypeText));
        }

        private void InteractWithItem(InteractItem interactItem)
        {
            attemptItemPickup = false;

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
            IInteractItem operation = interactItem.GetComponent<IInteractItem>();

            if (operation != null)
            {
                operation.InteractWith();
            }
        }

        private void InteractWithHoldItem(InteractItem interactItem)
        {
            EquipmentItem equipmentItem = interactItem.GetComponent<EquipmentItem>();

            if (equipmentItem != null)
            {
                equipmentItem.InteractWith();
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
                inventoryItem.InteractWith();
                OnResourceItemInteract?.Invoke(this, new ResourceItemEventArgs(inventoryItem));

                // Despawn Object
                Destroy(interactItem.transform.gameObject);
            }
        }

        RaycastHit[] RaycastAllSorted3D()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}