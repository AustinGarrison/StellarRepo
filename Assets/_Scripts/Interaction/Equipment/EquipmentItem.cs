using CallSOS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CallSOS.Player.Interaction.Equipment
{
    public class EquipmentItem : InteractItem, IRaycastable
    {
        [SerializeField] internal Sprite iconImage;
        [SerializeField] internal Vector3 holdPositionOffset;
        [SerializeField] internal EquipmentItemHandType handType;

        public bool isInInventory = false;
        public bool isInHand ;
        public bool debugOffest = false;

        internal EquipmentActions action;

        internal string mainActionText;
        internal string mainKeyText;

        internal string altActionText;
        internal string altKeyText;

        private void Start()
        {
            action = GetComponent<EquipmentActions>();
            GetActionsText();
        }

        private void Update()
        {
            if (isInInventory && debugOffest)
                transform.localPosition = holdPositionOffset;
        }

        private void GetActionsText()
        {
            string input = GetKeyDictionary(action.mainInputAction);
            mainActionText = "[" + input + "] " + action.mainActionText;

            if (action.altInputAction != null && action.altInputAction.bindings.Count > 0)
            {
               input = GetKeyDictionary(action.altInputAction);
                altActionText = "[" + input + "] " + action.altActionText;
            }
        }


        private string GetKeyDictionary(InputAction mainInputAction)
        {
            string bindingPath = mainInputAction.bindings[0].path;
            
            string label = "";

            if (InputsDictionary.PCKeyLabels.TryGetValue(bindingPath, out label))
            {
                return label;
            }
            else
            {

                return label;
            }
        }

        public override void InteractWith()//InteractControllerLocal player)
        {
            Debug.Log("HoldItem/InteractWith");
        }

        public CursorType GetCursorType()
        {
            return CursorType.Grab;
        }

        public bool CanHandleRaycast(Utilities.ObjectInteractController callingController)
        {
            return true;
        }

        public InteractItem GetInteractItem()
        {
            return GetComponent<InteractItem>();
        }
    }
}