using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractControllerLocal : MonoBehaviour
{
    [SerializeField] private Transform pickupPosition;
    [SerializeField] GameObject invDropItemObject;

    //[SerializeField] private KeyCode pickupKey = KeyCode.E;
    //[SerializeField] private KeyCode dropKey = KeyCode.G;
    //[SerializeField] private KeyCode toggleInventory = KeyCode.Tab;

    [SerializeField] GameObject crosshair;
    [SerializeField] TextMeshProUGUI holdText;
    [SerializeField] TextMeshProUGUI operationText;
    [SerializeField] TextMeshProUGUI pickupText;

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
        holdText.gameObject.SetActive(false);
        operationText.gameObject.SetActive(false);
        pickupText.gameObject.SetActive(false);
    }

    private void Start()
    {
        //Turns off inventory if its on
        if (invPanel.activeSelf)
            ToggleInventoryUI();
    }

    internal void Init()
    {
        GameInputPlayer.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInputPlayer.Instance.OnAltInteractAction += GameInput_OnAltInteractAction;
        GameInputPlayer.Instance.OnInventoryToggled += GameInput_OnInventoryToggled;
        _cameraTransform = Camera.main.transform;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        HandleScrolling();

        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
        {
            InteractItem item = hit.transform.gameObject.GetComponent<InteractItem>();
            InteractType type = item.type;

            switch (type)
            {
                case InteractType.OperationItem:
                    operationText.text = "[E] " + item.mouseOverText;
                    operationText.gameObject.SetActive(true);
                    break;
                case InteractType.HoldItem:
                    holdText.text = "[E] " + item.mouseOverText;
                    holdText.gameObject.SetActive(true);
                    break;
                case InteractType.InventoryItem:
                    pickupText.text = "[E] " + item.mouseOverText;
                    pickupText.gameObject.SetActive(true);
                    break;
            }
        }
        else
        {
            holdText.gameObject.SetActive(false);
            operationText.gameObject.SetActive(false);
            pickupText.gameObject.SetActive(false);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out RaycastHit hit, pickupRange, interactLayer))
        {
            InteractType type = hit.transform.gameObject.GetComponent<InteractItem>().type;

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
        //hit.transform.GetComponent<OperationItem>().InteractWith();
        IInteractItem operation = hit.transform.GetComponent<IInteractItem>();

        if(operation != null)
        {
            operation.InteractWith();
        }
    }

    private void PickupHeldItem(RaycastHit hit)
    {
        if (!holdingObject)
        {
            //hit.transform.GetComponent<HoldItem>().InteractWith();

            //Pickup Item
            SetObjectInHand(hit.transform.gameObject, pickupPosition.position, pickupPosition.rotation, gameObject);
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

        // Despawn Object
        Destroy(hit.transform.gameObject);
    }

    private void AddToinventory(Item newItem)
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

    private void DropHeldItem()
    {
        if (!holdingObject)
            return;

        DropObjectObserver(objectInHand, worldHeldItemHolder);
        holdingObject = false;
        objectInHand = null;
    }
    
    void SetObjectInHand(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.parent = player.transform;

        if (obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = true;

        if (obj.GetComponent<Collider>() != null)
            obj.GetComponent<Collider>().enabled = false;
        
    }

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

    void DropInventoryItem(Item item)
    {
        foreach (InventoryObject inventoryObject in inventoryObjects)
        {
            if (inventoryObject.item != item)
                continue;

            if(inventoryObject.amount > 1)
            {
                inventoryObject.amount--;
                DropItem(inventoryObject.item.prefab, inventoryObject.item.itemName, _cameraTransform.transform.position + _cameraTransform.transform.forward);
                UpdateInventoryUI();
                return;
            }

            if(inventoryObject.amount <= 1)
            {
                inventoryObjects.Remove(inventoryObject);
                DropItem(inventoryObject.item.prefab, inventoryObject.item.itemName, _cameraTransform.transform.position + _cameraTransform.transform.forward);
                UpdateInventoryUI();
                return;
            }
        }
    }

    void DropItem(GameObject prefab, string name, Vector3 position)
    {
        GameObject drop = Instantiate(prefab, position, Quaternion.identity, worldHeldItemHolder);
        drop.name = name;
    }

    private void HandleScrolling()
    {
        float scrollInput = GameInputPlayer.Instance.GetScrollAxis() / 1000;

        if (scrollInput > 0)
        {
            currentValue = (currentValue % MaxValue) + 1;
        }
        else if (scrollInput < 0)
        {
            currentValue = (currentValue - 2 + MaxValue) % MaxValue + 1;
        }
    }

    [System.Serializable]
    public class InventoryObject 
    {
        public Item item;
        public int amount;
    }
}
