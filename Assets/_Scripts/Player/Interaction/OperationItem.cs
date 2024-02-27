using UnityEngine;

public class OperationItem : InteractItem
{
    public MonoBehaviour uniqueOperation;
    public string interactText;

    public override void InteractWith(InteractControllerLocal player)
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("OperationItem/InteractWith");
    }
}
