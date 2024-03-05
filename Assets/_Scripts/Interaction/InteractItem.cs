using CallSOS.Player.Interaction;
using UnityEngine;

public enum InteractType
{
    HoldItem,
    InventoryItem,
    OperationItem
}

public class InteractItem : MonoBehaviour, IInteractItem
{

    public ItemSO itemScriptable;
    [HideInInspector] public InteractType interactType;

    public virtual void InteractWith() { }// InteractControllerLocal player) { }
}
