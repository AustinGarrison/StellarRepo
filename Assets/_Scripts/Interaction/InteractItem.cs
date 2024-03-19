using CallSOS.Player.Interaction;
using UnityEngine;

public enum InteractType
{
    HoldItem,
    ResourceItem,
    OperationItem
}

public class InteractItem : MonoBehaviour, IInteractItem
{

    public ItemSO itemScriptable;
    [HideInInspector] public InteractType interactType;

    public virtual void InteractWith() { }
}
