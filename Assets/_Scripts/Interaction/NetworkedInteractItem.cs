using CallSOS.Player.Interaction.Equipment;
using FishNet.Object;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedInteractItem : NetworkBehaviour
    {
        public virtual void NetworkedInteractWith() { }
        public virtual void NetworkedEquipmentInteractWith(Transform holdParent, Transform holdPosition, EquipmentItem equipmentItem) { }
        public virtual void SpawnItem(GameObject item) { }
    }
}