using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownInteractController : MonoBehaviour
{

    [SerializeField] private LayerMask interactLayer;

    private bool isInitialized;
    public bool pickupItem = false;

    internal void Initialize()
    {
        GameInputPlayer.Instance.OnClickAction += Instance_OnClickAction;
        isInitialized = true;
    }

    private void OnDisable()
    {
        GameInputPlayer.Instance.OnClickAction -= Instance_OnClickAction;
    }

    private void Instance_OnClickAction(object sender, System.EventArgs e)
    {
        Debug.Log("Clicked");
    }

    void Update()
    {
        if (!isInitialized) return;
    }
}
