using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigationOperation : InteractItem, IInteractItem
{
    public override void InteractWith(InteractControllerLocal player)
    {
        Debug.Log("I am the Navigation");
    }
}
