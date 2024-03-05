using CallSOS.Player.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigationOperation : InteractItem, IInteractItem
{
    private void Awake()
    {
        interactType = InteractType.OperationItem;
    }

    public override void InteractWith()//InteractControllerLocal player)
    {
        Debug.Log("I am the Navigation");
    }
}
