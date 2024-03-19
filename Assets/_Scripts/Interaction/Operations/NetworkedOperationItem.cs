using CallSOS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedOperationItem : NetworkedInteractItem, INetworkRaycastable
    {
        public OperationAction uniqueOperation;
        public InteractType awakeInteractType;
        public string interactText;
        public string localizationKey;

        private void Awake()
        {
            interactType = awakeInteractType;
        }

        public override void InteractWith()//InteractControllerLocal player)
        {
            Debug.Log("OperationItem/InteractWith");
            OperationAction action = GetComponent<OperationAction>();
            action.InteractWithOperation();
        }

        public bool CanHandleRaycast(ObjectInteractController callingController)
        {
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Point;
        }

        public NetworkedInteractItem GetInteractItem()
        {
            return GetComponent<NetworkedInteractItem>();
        }
    }
}