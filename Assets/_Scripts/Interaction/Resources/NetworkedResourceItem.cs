using FishNet;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedResourceItem : NetworkedInteractItem
    {
        public override void NetworkInteractWith()
        {
            ResourceItemInteractServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResourceItemInteractServerRPC()
        {
            ResourceItemInteractObserverRPC();
            ServerManager.Despawn(gameObject);
        }

        [ObserversRpc]
        private void ResourceItemInteractObserverRPC()
        {
            Debug.Log("Interact With");
        }

        public override void SpawnItem(GameObject item)
        {
        }       

    }
}