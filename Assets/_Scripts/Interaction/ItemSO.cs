using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public GameObject prefab;
    public string interactText;
}
