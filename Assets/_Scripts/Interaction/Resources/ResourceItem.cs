using CallSOS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    // Items that are not equipment. Can be dropped or brought back to player ship. Have no utility use.
    public class ResourceItem : InteractItem, IRaycastable
    {
        private void Awake()
        {
            interactType = InteractType.ResourceItem;
        }

        public override void InteractWith()
        {
            //base.InteractWith(); // Doesnt spawn player when active?
            Debug.Log("InventoryItem/InteractWith");

            // Despawn Object
            Destroy(gameObject);
        }

        public CursorType GetCursorType()
        {
            return CursorType.Grab;
        }

        public bool CanHandleRaycast(Utilities.ObjectInteractController callingController)
        {
            return true;
        }

        public InteractItem GetInteractItem()
        {
            return GetComponent<InteractItem>();
        }
    }
}