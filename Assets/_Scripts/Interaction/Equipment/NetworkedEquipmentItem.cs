using CallSOS.Player.Interaction;
using CallSOS.Player.Interaction.Equipment;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CallSOS.Player.Interaction
{
    public class NetworkedEquipmentItem : NetworkedInteractItem
    {
        public bool isInInventory = false;
        public bool isInHand;

        public override void NetworkedEquipmentInteractWith(Transform holdParent, Transform holdPosition, EquipmentItem equipmentItem)
        {
            Debug.Log(holdPosition);

            EquipmentPickupServerRpc(holdParent, holdPosition, equipmentItem.holdPositionOffset);

            //equipmentItem.action.UpdateIsInHand(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void EquipmentPickupServerRpc(Transform holdParent, Transform holdPosition, Vector3 offset)
        {
            EquipmentPickupObserversRpc(holdParent, holdPosition, offset);

        }

        [ObserversRpc]
        private void EquipmentPickupObserversRpc(Transform holdParent, Transform holdPosition, Vector3 offset)
        {

            transform.SetParent(holdParent);
            transform.parent = holdParent;
            transform.position = Vector3.zero;
            transform.localPosition = offset;
            //transform.rotation = holdPosition.rotation;

            if (GetComponent<Rigidbody>() != null)
                GetComponent<Rigidbody>().isKinematic = true;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;

            isInInventory = true;
        }
    }
}