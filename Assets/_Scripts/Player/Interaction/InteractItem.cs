using UnityEngine;

public enum InteractType
{
    HoldItem,
    InventoryItem,
    OperationItem
}

public class InteractItem : MonoBehaviour, IInteractItem
{
    public Item itemScriptable;
    public InteractType type;

    public virtual void InteractWith() { }
}
