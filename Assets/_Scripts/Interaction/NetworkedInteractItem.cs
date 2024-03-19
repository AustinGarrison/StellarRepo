using FishNet.Object;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedInteractItem : NetworkBehaviour
    {
        public virtual void NetworkInteractWith() { }
        public virtual void SpawnItem(GameObject item) { }
    }
}