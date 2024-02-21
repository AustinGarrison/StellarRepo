using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InteractControllerLocal;

namespace Player.Interaction
{
    public enum HoldItemHandType
    {
        OneHanded,
        TwoHanded
    }

    public class InventoryController : MonoBehaviour
    {
        [SerializeField] internal Transform worldHeldItemParent;
        [SerializeField] private InteractControllerLocal interactController;

        [SerializeField] private Transform holdPostion;

        [Header("HUD")]
        [SerializeField] private GameObject crosshair;
        [SerializeField] private GameObject UIInvObjectPrefab;
        [SerializeField] private GameObject invPanel;

        [SerializeField] private Transform UIinvObjectHolder;
        public List<ResourceObject> resourceObjects = new List<ResourceObject>();

        public InventorySlotInfo[] inventorySlots = new InventorySlotInfo[0];

        [System.Serializable]
        public struct InventorySlotInfo
        {
            public HoldItem heldItem;
            public InventorySlot slot;
        }

        // Scrolling
        [SerializeField] private float scrollingCooldown = 0.4f;
        private const int minScrollValue = 1;
        private const int maxScrollValue = 5;
        public int currentScrollValue = 1;
        public GameObject objectInHand;

        // Helpers
        bool isInCooldown = false;
        int twoHandedSlot = 5;

        internal void Init()
        {
            //Turns off inventory if its on
            if (invPanel.activeSelf)
                ToggleResourceUI();

            GameInputPlayer.Instance.OnResourceHUDToggled += GameInput_OnResourceHUDToggled;
            GameInputPlayer.Instance.OnAltInteractAction += GameInput_OnAltInteractAction;

            interactController.OnHoldItemInteract += InteractController_OnHoldItemInteract;
            interactController.OnInventoryItemInteract += InteractController_OnResourceItemInteract;
        }

        #region EventArgs

        private void GameInput_OnResourceHUDToggled(object sender, EventArgs e)
        {
            ToggleResourceUI();
        }

        private void GameInput_OnAltInteractAction(object sender, EventArgs e)
        {
            DropHeldItem();
        }

        private void InteractController_OnHoldItemInteract(object sender, HoldItemEventArgs e)
        {
            FindEmptyHotbarSlot(e.HoldItem);
        }

        private void InteractController_OnResourceItemInteract(object sender, InventoryItemEventArgs e)
        {
            AddResource(e.InventoryItem.itemScriptable);
        }

        #endregion

        private void Update()
        {
            GetScrollValue();
        }

        private void GetScrollValue()
        {
            if (isInCooldown) { return; }

            float scrollInput = GameInputPlayer.Instance.GetScrollAxis() / 1000;
            int previousValue = 0;

            if (scrollInput == 0) return;
            else if (scrollInput > 0)
            {
                previousValue = currentScrollValue;
                currentScrollValue = (currentScrollValue % maxScrollValue) + 1;

                ChangeHotbarSlot(previousValue);

            }
            else if (scrollInput < 0)
            {
                previousValue = currentScrollValue;
                currentScrollValue = (currentScrollValue - 2 + maxScrollValue) % maxScrollValue + 1;

                ChangeHotbarSlot(previousValue);
            }
        }

        private void ChangeHotbarSlot(int previousValue)
        {
            if (inventorySlots[currentScrollValue - 1].heldItem != null)
            {
                objectInHand = inventorySlots[currentScrollValue - 1].heldItem.gameObject;
            }
            if (inventorySlots[currentScrollValue - 1].heldItem == null)
            {
                objectInHand = null;
            }

            UpdateBothSlotUIHighlight(currentScrollValue, previousValue);
            DisableUnselectedItems();
            EnableCurrentItem();
            isInCooldown = true;

            StopCoroutine("ScrollCooldownTimer");
            StartCoroutine("ScrollCooldownTimer");
        }

        private void UpdateCurrentSlotUIHighlight(int currentValue)
        {
            inventorySlots[currentValue - 1].slot.StartCoroutine("SetHighlight");
        }

        private void UpdateBothSlotUIHighlight(int currentValue, int previousValue)
        {
            inventorySlots[previousValue - 1].slot.StartCoroutine("SetNormal");
            inventorySlots[currentValue - 1].slot.StartCoroutine("SetHighlight");
        }


        private void DisableUnselectedItems()
        {
            foreach (var item in inventorySlots)
            {
                if(item.heldItem != null)
                {
                    item.heldItem.gameObject.SetActive(false);
                }
            }
        }

        private void EnableCurrentItem()
        {
            if(inventorySlots[currentScrollValue - 1].heldItem  != null)
                inventorySlots[currentScrollValue - 1].heldItem.gameObject.SetActive(true);
        }

        internal void FindEmptyHotbarSlot(HoldItem item)
        {
            if (holdPostion == null)
            {
                Debug.LogError("No hold position found");
                return;
            }

            // Is item one or two handed
            if(item.handType == HoldItemHandType.OneHanded)
            {
                // Currently Selected slot is open, and not two handed slot
                if (inventorySlots[currentScrollValue - 1].heldItem == null && currentScrollValue != twoHandedSlot)
                {
                    inventorySlots[currentScrollValue - 1].heldItem = item;
                    inventorySlots[currentScrollValue - 1].slot.UpdateSlotIcons(item.iconImage);

                    // Set Slot Highlight, previous value doesnt matter
                    UpdateCurrentSlotUIHighlight(currentScrollValue);

                    MoveItemToPlayer(item);
                }
                else
                {
                    bool foundSlot = false;

                    // Loop through all slots to find first open
                    for (int slotIndex = 0; slotIndex < inventorySlots.Length; slotIndex++)
                    {
                        // Currently Selected slot is open
                        if (inventorySlots[slotIndex].heldItem == null)
                        {
                            inventorySlots[slotIndex].heldItem = item;
                            inventorySlots[slotIndex].slot.UpdateSlotIcons(item.iconImage);

                            int previousSlot = currentScrollValue;
                            currentScrollValue = slotIndex + 1;

                            // Change currenly selected to the new slot
                            ChangeHotbarSlot(previousSlot);

                            // Set Slot Highlight
                            UpdateBothSlotUIHighlight(currentScrollValue, previousSlot);

                            foundSlot = true;

                            MoveItemToPlayer(item);

                            // finished with loop
                            break;
                        }
                    }

                    // No Slot Found
                    if (!foundSlot)
                    {
                        Debug.Log("No Slot Found");

                        return;
                    }
                }
            }
            if (item.handType == HoldItemHandType.TwoHanded)
            {
                return;
            }
        }


        private void MoveItemToPlayer(HoldItem item)
        {
            item.transform.SetParent(holdPostion);
            item.transform.position = Vector3.zero;
            item.transform.localPosition = item.GetComponent<HoldItem>().holdPositionOffset;
            item.transform.rotation = holdPostion.rotation;

            item.isInInventory = true;

            if (item.GetComponent<Rigidbody>() != null)
                item.GetComponent<Rigidbody>().isKinematic = true;

            if (item.GetComponent<Collider>() != null)
                item.GetComponent<Collider>().enabled = false;

            objectInHand = item.transform.gameObject;
        }

        internal void DropHeldItem()
        {
            Debug.Log("Before if (!holdingObject)");

            if (objectInHand = null)
                return;

            Debug.Log("After if (!holdingObject)");

            //HoldItem holdItem = objectInHand.GetComponent<HoldItem>();
            HoldItem holdItem = inventorySlots[currentScrollValue - 1].heldItem;
            inventorySlots[currentScrollValue - 1].heldItem = null;

            inventorySlots[currentScrollValue - 1].slot.DisableSlotIcons();

            holdItem.transform.parent = worldHeldItemParent;
            holdItem.isInInventory = false;

            if (holdItem.GetComponent<Rigidbody>() != null)
                holdItem.GetComponent<Rigidbody>().isKinematic = false;

            if (holdItem.GetComponent<Collider>() != null)
                holdItem.GetComponent<Collider>().enabled = true;

            objectInHand = null;
        }

        IEnumerator ScrollCooldownTimer()
        {
            yield return new WaitForSecondsRealtime(scrollingCooldown);
            isInCooldown = false;
        }

        #region Resources
        internal void UpdateResourceList()
        {
            foreach (Transform child in UIinvObjectHolder)
                Destroy(child.gameObject);

            foreach (ResourceObject inventoryObject in resourceObjects)
            {
                GameObject obj = Instantiate(UIInvObjectPrefab, UIinvObjectHolder);
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryObject.item.itemName + " - " + inventoryObject.amount;
                obj.GetComponent<Button>().onClick.AddListener(delegate { DropResourceItem(inventoryObject.item); });
            }
        }

        internal void ToggleResourceUI()
        {
            if (invPanel.activeSelf)
            {
                invPanel.SetActive(false);
                crosshair.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!invPanel.activeSelf)
            {

                UpdateResourceList();
                invPanel.SetActive(true);
                crosshair.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        internal void AddResource(ItemSO newItem)
        {
            foreach (ResourceObject inventoryObject in resourceObjects)
            {
                if (inventoryObject.item == newItem)
                {
                    inventoryObject.amount++;
                    return;
                }
            }

            resourceObjects.Add(new ResourceObject() { item = newItem, amount = 1 });
        }

        void DropResourceItem(ItemSO item)
        {
            foreach (ResourceObject resourceObject in resourceObjects)
            {
                if (resourceObject.item != item)
                    continue;

                if (resourceObject.amount > 1)
                {
                    resourceObject.amount--;
                    DropResource(resourceObject, transform.position + transform.forward);
                    UpdateResourceList();
                    return;
                }

                if (resourceObject.amount <= 1)
                {
                    resourceObjects.Remove(resourceObject);
                    DropResource(resourceObject, transform.position + transform.forward);
                    UpdateResourceList();
                    return;
                }
            }
        }

        void DropResource(ResourceObject itemSO, Vector3 position)
        {
            GameObject drop = Instantiate(itemSO.item.prefab, position, Quaternion.identity, worldHeldItemParent);
            drop.name = itemSO.item.name;
            drop.GetComponent<InventoryItem>().itemScriptable.interactText = itemSO.item.interactText;
        }
        #endregion
    }

    [System.Serializable]
    public class ResourceObject
    {
        public ItemSO item;
        public int amount;
    }
}