using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigationOperation : InteractItem, IInteractItem
{
    public override void InteractWith()
    {
        Debug.Log("I am the Navigation");
    }
}
