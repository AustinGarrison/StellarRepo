using CallSOS.Player.Interaction;
using CallSOS.Utilities;
using UnityEngine;

public class OperationItem : InteractItem, IRaycastable
{
    public OperationAction localOperation;
    public NetworkOperationAction networkOperation;
    public InteractType awakeInteractType;
    public string interactText;
    public string localizationKey;


    private void Awake()
    {
        interactType = awakeInteractType;

        OperationAction action;
        if (TryGetComponent(out action))
        {
            localOperation = action;
        }
        else
        {
            Debug.LogError("OperationAction component not found!");
        }

        NetworkOperationAction networkAction;
        if (TryGetComponent(out networkAction))
        {
            networkOperation = networkAction;
        }
        else
        {
            Debug.LogError("NetworkOperationAction component not found!");
        }
    }

    public override void InteractWith()//InteractControllerLocal player)
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("OperationItem/InteractWith");

        if(localOperation != null)
            localOperation.InteractWithOperation();

        if (networkOperation != null)
            networkOperation.InteractWithOperation();
    }

    public CursorType GetCursorType()
    {
        return CursorType.Point;
    }

    public bool CanHandleRaycast(ObjectInteractController callingController)
    {
        return true;
    }

    public InteractItem GetInteractItem()
    {
        return GetComponent<InteractItem>();
    }
}
