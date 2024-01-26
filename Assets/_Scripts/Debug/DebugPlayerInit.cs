using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugPlayerInit : MonoBehaviour
{
    public UnityEvent InitPlayer;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10)) 
        {
            InitPlayer.Invoke();
            Destroy(this);
        }
    }
}
