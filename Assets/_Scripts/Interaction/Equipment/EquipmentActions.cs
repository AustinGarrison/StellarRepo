using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CallSOS.Player.Interaction.Equipment
{
    public class EquipmentActions : MonoBehaviour
    {
        public InputAction mainInputAction;
        public InputAction altInputAction;

        [SerializeField] internal string mainActionText;
        [SerializeField] internal string altActionText;
        public string MainActionText { get; private set; }
        public string AltActionText { get; private set; }

        public bool isInHand = false;

        private void Start()
        {
            mainInputAction?.Enable();
            altInputAction?.Enable();
        }

        /// <summary>
        /// Listen for main equipment hotkey or optional alternate hotkey
        /// Dont listen for input if item is not in players hand
        /// </summary>
        private void Update()
        {
            if (!isInHand) return;

            if (mainInputAction.triggered) EquipmentMainAbility();

            if (altInputAction.triggered) EquipmentAltAbility();

        }

        internal void UpdateIsInHand(bool handStatus)
        {
            isInHand = handStatus;
        }

        public string GetMainAbilityText()
        {
            return mainActionText;
        }

        public string GetAltAbilityText()
        {

            return altActionText;
        }

        public virtual void EquipmentMainAbility() { }
        public virtual void EquipmentAltAbility() { }
    }
}