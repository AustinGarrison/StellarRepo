using Player.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HoldItem : InteractItem
{
    [SerializeField] internal Sprite iconImage;
    [SerializeField] internal Vector3 holdPositionOffset;
    [SerializeField] internal HoldItemHandType handType;
    public bool isInInventory = false;

    private void Update()
    {
        if (isInInventory)
            transform.localPosition = holdPositionOffset;
    }

    private void Awake()
    {
        interactType = InteractType.HoldItem;
    }

    public override void InteractWith(InteractControllerLocal player)
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("HoldItem/InteractWith");
    }
}
