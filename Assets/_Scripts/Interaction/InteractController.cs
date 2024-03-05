using FishNet.Object;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractController : NetworkBehaviour
{
    [SerializeField] private Transform pickupPosition;
    [SerializeField] GameObject invDropItemObject;

    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject holdText;
    [SerializeField] GameObject interactText;
    [SerializeField] GameObject pickupText;

    [SerializeField] private float pickupRange = 4f;

    [SerializeField] private LayerMask interactLayer;

    public List<InventoryObject> inventoryObjects = new List<InventoryObject>();
    [SerializeField] private GameObject invPanel;
    [SerializeField] private Transform invObjectHolder;
    [SerializeField] private Transform _cameraTransform;
    private Transform worldHeldItemHolder;
    private GameObject objectInHand;
    private bool holdingObject;
    private bool initialized;

    //Inventory scrolling
    private const int MinValue = 1;
    private const int MaxValue = 5;
    private int currentValue = 1;

    private void Awake()
    {
        holdText.SetActive(false);
        interactText.SetActive(false);
        pickupText.SetActive(false);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        //worldHeldItemHolder = GameObject.FindGameObjectWithTag("WorldInteractItems").transform;

        //Turns off inventory if its on
        if (invPanel.activeSelf)
            ToggleInventoryUI();
    }

    internal void Init()
    {
        GameInputPlayer.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInputPlayer.Instance.OnAltInteractAction += GameInput_OnAltInteractAction;
        GameInputPlayer.Instance.OnResourceHUDToggled += GameInput_OnInventoryToggled;
        _cameraTransform = Camera.main.transform;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        HandleScrolling();

        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
        {
            InteractType type = hit.transform.gameObject.GetComponent<InteractItem>().interactType;

            switch (type)
            {
                case InteractType.OperationItem:
                    interactText.SetActive(true);
                    break;
                case InteractType.HoldItem:
                    holdText.SetActive(true);
                    break;
                case InteractType.InventoryItem:
                    pickupText.SetActive(true);
                    break;
            }
        }
        else
        {
            holdText.SetActive(false);
            interactText.SetActive(false);
            pickupText.SetActive(false);
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
                    TriggerOperationItem(hit);
                    break;
                case InteractType.HoldItem:
                    PickupHeldItem(hit);
                    break;
                case InteractType.InventoryItem:
                    PickupInventoryItem(hit);
                    break;
                default:
                    Debug.LogError("No interaction type found in Hit");
                    break;
            }
        }
    }

    private void GameInput_OnAltInteractAction(object sender, System.EventArgs e)
    {
        DropHeldItem();
    }

    private void GameInput_OnInventoryToggled(object sender, System.EventArgs e)
    {
        ToggleInventoryUI();
    }

    private void TriggerOperationItem(RaycastHit hit)
    {
        // Disabled because Interactwith takes in (this) which is current a non networked InteractController
        //hit.transform.GetComponent<OperationItem>().InteractWith();
    }

    private void PickupHeldItem(RaycastHit hit)
    {
        if (!holdingObject)
        {
            //hit.transform.GetComponent<HoldItem>().InteractWith();

            //Pickup Item
            SetObjectInHandServer(hit.transform.gameObject, pickupPosition.position, pickupPosition.rotation, gameObject);
            objectInHand = hit.transform.gameObject;
            holdingObject = true;
        }
        else if (holdingObject)
        {
            //Find available slot
        }
    }

    private void PickupInventoryItem(RaycastHit hit)
    {
        //if (hit.transform.GetComponent<InventoryItem>() == null)
        //    return;

        //hit.transform.GetComponent<InventoryItem>().InteractWith();

        AddToinventory(hit.transform.GetComponent<InteractItem>().itemScriptable);

        DespawnObjectServer(hit.transform.gameObject);
    }

    private void AddToinventory(ItemSO newItem)
    {
        foreach (InventoryObject inventoryObject in inventoryObjects)
        {
            if(inventoryObject.item == newItem)
            {
                inventoryObject.amount++;
                return;
            }
        }

        inventoryObjects.Add(new InventoryObject() { item = newItem, amount = 1 });
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnObjectServer(GameObject objectToDespawn)
    {
        ServerManager.Despawn(objectToDespawn, DespawnType.Destroy);
    }

    private void DropHeldItem()
    {
        if (!holdingObject)
            return;

        DropObjectServer(objectInHand, worldHeldItemHolder);
        holdingObject = false;
        objectInHand = null;
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

    [ServerRpc(RequireOwnership = false)]
    private void DropObjectServer(GameObject obj, Transform worldObjects)
    {
        DropObjectObserver(obj, worldObjects);
    }

    [ObserversRpc]
    private void DropObjectObserver(GameObject obj, Transform worldObjects)
    {
        obj.transform.parent = worldObjects;

        if(obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent <Rigidbody>().isKinematic = false;

        if (obj.GetComponent<Collider>() != null)
            obj.GetComponent<Collider>().enabled = true;
    }

    void ToggleInventoryUI()
    {
        if(invPanel.activeSelf)
        {
            invPanel.SetActive(false);
            crosshair.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!invPanel.activeSelf)
        {

            UpdateInventoryUI();
            invPanel.SetActive(true);
            crosshair.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void UpdateInventoryUI()
    {
        foreach (Transform child in invObjectHolder)
            Destroy(child.gameObject);

        foreach (InventoryObject inventoryObject in inventoryObjects)
        {
            GameObject obj = Instantiate(invDropItemObject, invObjectHolder);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryObject.item.itemName + " - " + inventoryObject.amount;
            obj.GetComponent<Button>().onClick.AddListener(delegate { DropInventoryItem(inventoryObject.item); });
        }
    }

    void DropInventoryItem(ItemSO item)
    {

        foreach (InventoryObject inventoryObject in inventoryObjects)
        {
            if (inventoryObject.item != item)
                continue;

            if(inventoryObject.amount > 1)
            {
                inventoryObject.amount--;
                DropItemServer(inventoryObject.item.prefab, inventoryObject.item.itemName, _cameraTransform.transform.position + _cameraTransform.transform.forward);
                UpdateInventoryUI();
                return;
            }

            if(inventoryObject.amount <= 1)
            {
                inventoryObjects.Remove(inventoryObject);
                DropItemServer(inventoryObject.item.prefab, inventoryObject.item.itemName, _cameraTransform.transform.position + _cameraTransform.transform.forward);
                UpdateInventoryUI();
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DropItemServer(GameObject prefab, string name, Vector3 position)
    {
        GameObject drop = Instantiate(prefab, position, Quaternion.identity, worldHeldItemHolder);
        drop.name = name;
        ServerManager.Spawn(drop);
    }

    private void HandleScrolling()
    {
        float scrollInput = GameInputPlayer.Instance.GetScrollAxis() / 1000;

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

    [System.Serializable]
    public class InventoryObject 
    {
        public ItemSO item;
        public int amount;
    }
}
