using CallSOS.Utilities;
using FishNet.Managing.Server;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    // Items that are not equipment. Can be dropped or brought back to player ship. Have no utility use.
    public class ResourceItem : InteractItem, IRaycastable
    {
        public InteractType awakeInteractType;

        public int amount;
        public bool isNetworked;
        [SerializeField] internal NetworkedInteractItem networkedResource;

        private void Awake()
        {
            interactType = awakeInteractType;
        }

        public override void InteractWith()
        {
            //base.InteractWith(); // Doesnt spawn player when active?
            Debug.Log("InventoryItem/InteractWith");

            if(isNetworked && networkedResource != null)
                networkedResource.NetworkInteractWith();
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