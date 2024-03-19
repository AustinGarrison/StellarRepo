using UnityEngine;

public class NetworkedInteractItem : MonoBehaviour, IInteractItem
{
    public ItemSO itemScriptable;
    [HideInInspector] public InteractType interactType;

    public virtual void InteractWith() { }
}
