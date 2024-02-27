using CallSOS.Player.Interaction;
using UnityEngine;

public class ShipPrinterOperation : InteractItem, IInteractItem
{
    private void Awake()
    {
        interactType = InteractType.OperationItem;
    }

    public override void InteractWith(InteractControllerLocal player)
    {
        Debug.Log("I am a 3d Printer!");
    }
}
