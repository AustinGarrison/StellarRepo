using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Interaction
{
    // Items that are not equipment. Can be dropped or brought back to player ship. Have no utility use.
    public class InventoryItem : InteractItem
    {
        private void Awake()
        {
            interactType = InteractType.InventoryItem;
        }
        public override void InteractWith(InteractControllerLocal player)
        {
            //base.InteractWith(); // Doesnt spawn player when active?
            Debug.Log("InventoryItem/InteractWith");
        }
    }
}