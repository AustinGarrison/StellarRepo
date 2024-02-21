using UnityEngine;

public enum InteractType
{
    HoldItem,
    InventoryItem,
    OperationItem
}

public class InteractItem : MonoBehaviour, IInteractItem
{

    [ReadOnlyRunTime] public ItemSO itemScriptable;
    [HideInInspector] public InteractType interactType;

    public virtual void InteractWith(InteractControllerLocal player) { }
}
