using CallSOS.Player.Interaction;
using CallSOS.Utilities;
using UnityEngine;

public class OperationItem : InteractItem, IRaycastable
{
    public OperationAction uniqueOperation;
    public string interactText;
    public string localizationKey;


    private void Awake()
    {
        interactType = InteractType.OperationItem;
    }

    public override void InteractWith()//InteractControllerLocal player)
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("OperationItem/InteractWith");
        OperationAction action = GetComponent<OperationAction>();
        action.InteractWithOperation();
    }

    public CursorType GetCursorType()
    {
        return CursorType.Point;
    }

    public bool CanHandleRaycast(CallSOS.Utilities.ObjectInteractController callingController)
    {
        return true;
    }

    public InteractItem GetInteractItem()
    {
        return GetComponent<InteractItem>();
    }
}
