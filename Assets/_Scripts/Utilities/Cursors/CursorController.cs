using CallSOS.Player.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CallSOS.Player.Interaction.InteractControllerLocal;

namespace CallSOS.Utilities
{
    public enum CursorType
    {
        None,
        UI,
        Point,
        Open,
        Grab,
    }

    public class CursorController : MonoBehaviour
    {
        [System.Serializable]
        struct CursorMapping
        {
            public string cursorName;
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float raycastRadius = 1f;

        public bool useCursors = false;
        private bool isInitialized = false;
        private bool canInteractItem = false;

        #region Events

        // Events
        public event EventHandler<EquipmentItemEventArgs> OnEquipmentItemInteract;
        public event EventHandler<ResourceItemEventArgs> OnResourceItemInteract;
        public event EventHandler<ChangeTextEvent> OnInteractTextEvent;
        
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
            canInteractItem = true;
        }

        private void Update()
        {
            if (!isInitialized) return; 
            if (InteractWithUI()) return;
            if (InteractWithComponent3D()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                SetCursor(CursorType.UI);

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
                        SetCursor(raycastable.GetCursorType());

                        InteractItem interactItem = raycastable.GetInteractItem();

                        if (interactItem != null)
                        {
                            SetPickupText(interactItem);

                            if (canInteractItem == true)
                                InteractWithItem(interactItem);
                        }

                        return true;
                    }
                }
            }

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

        private void InteractWithItem(InteractItem interactItem)
        {
            canInteractItem = false;
            Debug.Log("Attempt Item");

            //InteractType type = interactItem.interactType;

            //switch (type)
            //{
            //    case InteractType.OperationItem:
            //        InteractWithOperationItem(interactItem);
            //        interactItem.InteractWith();
            //        break;
            //    case InteractType.HoldItem:
            //        InteractWithHoldItem(interactItem);
            //        break;
            //    case InteractType.InventoryItem:
            //        InteractWithInventoryItem(interactItem);
            //        break;
            //    default:
            //        Debug.LogError("No interaction type found in Hit");
            //        break;
            //}
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

        private void SetCursor(CursorType type)
        {
            if (useCursors)
            {
                CursorMapping mapping = GetCursorMapping(type);
                Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
            }
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}