using UnityEngine;

namespace CallSOS.Player.Interaction.Equipment
{
    public class EquipmentItem : InteractItem
    {
        [SerializeField] internal Sprite iconImage;
        [SerializeField] internal Vector3 holdPositionOffset;
        [SerializeField] internal EquipmentItemHandType handType;

        public bool isInInventory = false;
        public bool isInHand ;
        public bool debugOffest = false;

        internal string mainAbilityText;
        internal string altAbilityText;
        public EquipmentActions action;

        private void Start()
        {
            action = GetComponent<EquipmentActions>();
        }

        private void Update()
        {
            if (isInInventory && debugOffest)
                transform.localPosition = holdPositionOffset;
        }

        public override void InteractWith(InteractControllerLocal player)
        {
            Debug.Log("HoldItem/InteractWith");
        }
    }
}