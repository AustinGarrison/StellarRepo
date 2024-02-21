using FishNet.Demo.AdditiveScenes;
using Player.Interaction;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractControllerLocal : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private float pickupRange = 4f;
    [SerializeField] private LayerMask interactLayer;

    private Transform _cameraTransform;
    private bool isInitialized;

    // Events
    public event EventHandler<HoldItemEventArgs> OnHoldItemInteract;
    public event EventHandler<InventoryItemEventArgs> OnInventoryItemInteract;
    public class HoldItemEventArgs : EventArgs
    {
        public HoldItem HoldItem { get; }

        public HoldItemEventArgs(HoldItem item)
        {
            HoldItem = item;
        }
    }

    public class InventoryItemEventArgs : EventArgs
    {
        public InventoryItem InventoryItem { get; }

        public InventoryItemEventArgs(InventoryItem item)
        {
            InventoryItem = item;
        }
    }

    private void Awake()
    {
        interactText.gameObject.SetActive(false);
    }

    internal void Init()
    {
        GameInputPlayer.Instance.OnInteractAction += GameInput_OnInteractAction;
        _cameraTransform = Camera.main.transform;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
        {
            InteractItem item = hit.transform.gameObject.GetComponent<InteractItem>();
            InteractType type = item.interactType;

            switch (type)
            {
                case InteractType.OperationItem:
                    OperationItem opItem = item.GetComponentInParent<OperationItem>();
                    
                    if (opItem != null)
                        interactText.text = "[E] " + opItem.interactText;
                    else
                        interactText.text = "[E] " + "Interact";
                    
                    interactText.gameObject.SetActive(true);
                    break;
                case InteractType.HoldItem:
                    interactText.text = "[E] " + item.itemScriptable.interactText;
                    interactText.gameObject.SetActive(true);
                    break;
                case InteractType.InventoryItem:
                    interactText.text = "[E] " + item.itemScriptable.interactText;
                    interactText.gameObject.SetActive(true);
                    break;
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
        {
            InteractType type = hit.transform.gameObject.GetComponent<InteractItem>().interactType;

            switch (type)
            {
                case InteractType.OperationItem:
                    InteractWithOperationItem(hit);
                    break;
                case InteractType.HoldItem:
                    InteractWithHoldItem(hit);
                    break;
                case InteractType.InventoryItem:
                    InteractWithInventoryItem(hit);
                    break;
                default:
                    Debug.LogError("No interaction type found in Hit");
                    break;
            }
        }
    }

    private void InteractWithOperationItem(RaycastHit hit)
    {
        //hit.transform.GetComponent<OperationItem>().InteractWith();
        IInteractItem operation = hit.transform.GetComponent<IInteractItem>();

        if(operation != null)
        {
            operation.InteractWith(this);
        }
    }

    private void InteractWithHoldItem(RaycastHit hit)
    {
        HoldItem holdItem = hit.transform.GetComponent<HoldItem>();

        if(holdItem != null)
        {
            holdItem.InteractWith(this);

            OnHoldItemInteract?.Invoke(this, new HoldItemEventArgs(holdItem));
        }
        else
        {
            Debug.LogWarning("Failed to get hold Item");
        }
    }

    private void InteractWithInventoryItem(RaycastHit hit)
    {
        //if (hit.transform.GetComponent<InventoryItem>() == null)
        //    return;
        InventoryItem inventoryItem = hit.transform.GetComponent<InventoryItem>();

        if(inventoryItem != null)
        {
            inventoryItem.InteractWith(this);
            OnInventoryItemInteract?.Invoke(this, new InventoryItemEventArgs(inventoryItem));

            // Despawn Object
            Destroy(hit.transform.gameObject);
        }
    }
}
