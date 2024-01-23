using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItem : InteractItem
{
    public override void InteractWith()
    {
        //base.InteractWith(); // Doesnt spawn player when active?
        Debug.Log("HoldItem/InteractWith");
    }
}
