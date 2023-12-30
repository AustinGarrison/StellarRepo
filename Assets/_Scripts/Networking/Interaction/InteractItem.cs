using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractType
{
    HoldItem,
    InventoryItem,
    InteractItem
}

public class InteractItem : MonoBehaviour
{
    public Item itemScriptable;
    public InteractType type;
}
