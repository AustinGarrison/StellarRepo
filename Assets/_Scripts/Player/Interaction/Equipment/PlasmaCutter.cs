using UnityEngine;

namespace CallSOS.Player.Interaction.Equipment
{
    public class PlasmaCutter : EquipmentActions
    {
        public override void EquipmentMainAbility()
        {
            Debug.Log("Plasma Cutter cutting");
        }

        public override void EquipmentAltAbility()
        {
            Debug.Log("Hand Status: " + isInHand);
        }
    }
}