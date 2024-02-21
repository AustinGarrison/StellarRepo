using UnityEngine;

public class ShipPrinterOperation : InteractItem, IInteractItem
{
    public override void InteractWith(InteractControllerLocal player)
    {
        Debug.Log("I am a 3d Printer!");
    }
}
