using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedEquipmentItem : NetworkedInteractItem
    {
        public override void NetworkInteractWith()
        {
            EquipmentItemInteractServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void EquipmentItemInteractServerRPC()
        {
            EquipmentItemInteractObserverRPC();
        }

        [ObserversRpc]
        private void EquipmentItemInteractObserverRPC()
        {
            Debug.Log("Interact With");
        }
        public override void SpawnItem(GameObject item)
        {
        }
    }
}