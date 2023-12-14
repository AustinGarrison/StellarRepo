using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : NetworkBehaviour
{
    [SerializeField] private Transform pickupPosition;

    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private float pickupRange = 4f;

    [SerializeField] private LayerMask pickupHeldItemLayer;

    private Transform _cameraTransform;
    private bool holdingObject;
    private GameObject objectInHand;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!IsOwner)
        {
            enabled = false;
            return;
        }

        _cameraTransform = Camera.main.transform;
    }

    private const int MinValue = 1;
    private const int MaxValue = 5;

    private int currentValue = 1;

    internal void HandleInteraction()
    {
        HandlePickup();
        HandleScrolling();
    }

    private void HandlePickup()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            PickupHeldItem();
        }
    }

    private void PickupHeldItem()
    {
        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, pickupHeldItemLayer))
        {
            if (!holdingObject)
            {
                //Pickup Item
                SetObjectInHandServer(hit.transform.gameObject, pickupPosition.position, pickupPosition.rotation, gameObject);
                objectInHand = hit.transform.gameObject;
                holdingObject = true;
            }
            else if(holdingObject)
            {
                //Find available slot
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetObjectInHandServer(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
    {
        SetObjectInHandObserver(obj, position, rotation, player);
    }

    [ObserversRpc]
    void SetObjectInHandObserver(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.parent = player.transform;

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = true;

        if (obj.GetComponent<Collider>() != null)
            obj.GetComponent<Collider>().enabled = false;
        
    }


    private void HandleScrolling()
    {

        float scrollInput = GameInput.Instance.GetScrollAxis() / 1000;

        if (scrollInput > 0)
        {
            currentValue = (currentValue % MaxValue) + 1;

            Debug.Log("Current Value: " + currentValue);
        }
        else if (scrollInput < 0)
        {
            currentValue = (currentValue - 2 + MaxValue) % MaxValue + 1;

            Debug.Log("Current Value: " + currentValue);
        }
    }
}
