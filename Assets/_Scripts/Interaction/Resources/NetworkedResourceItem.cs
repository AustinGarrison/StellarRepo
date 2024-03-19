using CallSOS.Player.Interaction;
using CallSOS.Utilities;
using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedResourceItem : NetworkedInteractItem, INetworkRaycastable
    {
        public InteractType awakeInteractType;

        private void Awake()
        {
            interactType = awakeInteractType;
        }

        public override void InteractWith()
        {
            Debug.Log("NetworkInteractWith");
            //Destroy(gameObject);
        }

        public bool CanHandleRaycast(ObjectInteractController callingController)
        {
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Grab;
        }

        public NetworkedInteractItem GetInteractItem()
        {
            return GetComponent<NetworkedInteractItem>();
        }
    }
}