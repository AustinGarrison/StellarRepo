using UnityEngine;

namespace CallSOS.Player.Interaction.Equipment
{
    public class Flashlight : EquipmentActions
    {
        private bool flashlightStatus = false;

        public override void EquipmentMainAbility()
        {
            if (flashlightStatus)
            {
                flashlightStatus = false;
                Debug.Log("Flashlight Off");
                return;
            }
            else
            {
                flashlightStatus = true;
                Debug.Log("Flashlight On");
            }
        }

        public override void EquipmentAltAbility()
        {
            Debug.Log("Alt Ability");
        }
    }
}