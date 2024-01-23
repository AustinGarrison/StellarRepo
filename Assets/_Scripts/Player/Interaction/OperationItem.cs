using UnityEngine;

public class OperationItem : InteractItem
{
    public override void InteractWith()
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("OperationItem/InteractWith");
    }
}
