using CallSOS.Player.Interaction.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CallSOS.Player.Interaction
{
    public enum EquipmentItemHandType
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

        [Header("Resource and Equipment Holders")]
        public List<ResourceObject> resourceObjects = new List<ResourceObject>();
        public InventorySlotInfo[] hotbarSlots = new InventorySlotInfo[0];
        public EquipmentItem objectInHand;

        [System.Serializable]
        public struct InventorySlotInfo
        {
            public EquipmentItem heldItem;
            public InventorySlot slot;
        }

        [Header("Scrolling")]
        [SerializeField] private float scrollingCooldown = 0.4f;
        private const int minScrollValue = 1;
        private const int maxScrollValue = 5;
        public int currentScrollValue = 1;

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

            interactController.OnEquipmentItemInteract += InteractController_OnHoldItemInteract;
            interactController.OnResourceItemInteract += InteractController_OnResourceItemInteract;
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

        private void InteractController_OnHoldItemInteract(object sender, InteractControllerLocal.EquipmentItemEventArgs e)
        {
            FindEmptyHotbarSlot(e.EquipmentItem);
        }

        private void InteractController_OnResourceItemInteract(object sender, InteractControllerLocal.ResourceItemEventArgs e)
        {
            AddResource(e.ResourceItem.itemScriptable);
        }

        #endregion

        private void Update()
        {
            GetScrollValue();
        }

        private void GetScrollValue()
        {
            if (isInCooldown) return;

            float scrollInput = GameInputPlayer.Instance.GetScrollAxis() / 1000;

            if (scrollInput == 0) return;

            int previousValue = currentScrollValue;

            if (scrollInput > 0)
            {
                currentScrollValue = (currentScrollValue % maxScrollValue) + 1;
            }
            else if (scrollInput < 0)
            {
                currentScrollValue = (currentScrollValue - 2 + maxScrollValue) % maxScrollValue + 1;
            }

            ChangeHotbarSlot(previousValue);
        }

        private void ChangeHotbarSlot(int previousValue)
        {
            SetActiveItem();

            UpdateBothSlotUIHighlight(currentScrollValue, previousValue);
            DisableUnselectedItems();
            EnableCurrentItem();
            isInCooldown = true;

            StopCoroutine("ScrollCooldownTimer");
            StartCoroutine("ScrollCooldownTimer");
        }

        private void SetActiveItem()
        {
            if(objectInHand != null)
                objectInHand.action.UpdateIsInHand(false);

            if (hotbarSlots[currentScrollValue - 1].heldItem != null)
            {
                objectInHand = hotbarSlots[currentScrollValue - 1].heldItem;
                objectInHand.action.UpdateIsInHand(true);
            }
                
            if (hotbarSlots[currentScrollValue - 1].heldItem == null)
            {
                objectInHand = null;
            }   
        }

        private void UpdateCurrentSlotUIHighlight(int currentValue)
        {
            hotbarSlots[currentValue - 1].slot.StartCoroutine("SetHighlight");
        }

        private void UpdateBothSlotUIHighlight(int currentValue, int previousValue)
        {
            hotbarSlots[previousValue - 1].slot.StartCoroutine("SetNormal");
            hotbarSlots[currentValue - 1].slot.StartCoroutine("SetHighlight");
        }


        private void DisableUnselectedItems()
        {
            foreach (var item in hotbarSlots)
            {
                if(item.heldItem != null)
                {
                    item.heldItem.gameObject.SetActive(false);
                }
            }
        }

        private void EnableCurrentItem()
        {
            if(hotbarSlots[currentScrollValue - 1].heldItem  != null)
                hotbarSlots[currentScrollValue - 1].heldItem.gameObject.SetActive(true);
        }

        internal void FindEmptyHotbarSlot(EquipmentItem item)
        {
            if (holdPostion == null)
            {
                Debug.LogError("No hold position found");
                return;
            }

            // Is item one or two handed
            if(item.handType == EquipmentItemHandType.OneHanded)
            {
                // Currently Selected slot is open, and not two handed slot
                if (hotbarSlots[currentScrollValue - 1].heldItem == null && currentScrollValue != twoHandedSlot)
                {
                    hotbarSlots[currentScrollValue - 1].heldItem = item;
                    hotbarSlots[currentScrollValue - 1].slot.UpdateSlotIcons(item.iconImage);

                    // Set Slot Highlight, previous value doesnt matter
                    UpdateCurrentSlotUIHighlight(currentScrollValue);

                    MoveItemToPlayerHand(item);
                }
                else
                {
                    bool foundSlot = false;

                    // Loop through all slots to find first open
                    for (int slotIndex = 0; slotIndex < hotbarSlots.Length; slotIndex++)
                    {
                        // Currently Selected slot is open
                        if (hotbarSlots[slotIndex].heldItem == null)
                        {
                            hotbarSlots[slotIndex].heldItem = item;
                            hotbarSlots[slotIndex].slot.UpdateSlotIcons(item.iconImage);

                            int previousSlot = currentScrollValue;
                            currentScrollValue = slotIndex + 1;

                            // Change currenly selected to the new slot
                            ChangeHotbarSlot(previousSlot);

                            // Set Slot Highlight
                            UpdateBothSlotUIHighlight(currentScrollValue, previousSlot);

                            foundSlot = true;

                            MoveItemToPlayerHand(item);

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
            if (item.handType == EquipmentItemHandType.TwoHanded)
            {
                if (hotbarSlots[twoHandedSlot - 1].heldItem == null)
                {
                    hotbarSlots[twoHandedSlot - 1].heldItem = item;
                    hotbarSlots[twoHandedSlot - 1].slot.UpdateSlotIcons(item.iconImage);

                    // Set Slot Highlight, previous value doesnt matter
                    UpdateCurrentSlotUIHighlight(twoHandedSlot);

                    MoveItemToPlayerHand(item);
                }
                else
                {
                    Debug.Log("Two Handed Is Full");
                }
            }
        }

        private void MoveItemToPlayerHand(EquipmentItem equipmentItem)
        {
            equipmentItem.transform.SetParent(holdPostion);
            equipmentItem.transform.position = Vector3.zero;
            equipmentItem.transform.localPosition = equipmentItem.GetComponent<EquipmentItem>().holdPositionOffset;
            equipmentItem.transform.rotation = holdPostion.rotation;

            equipmentItem.isInInventory = true;

            if (equipmentItem.GetComponent<Rigidbody>() != null)
                equipmentItem.GetComponent<Rigidbody>().isKinematic = true;

            if (equipmentItem.GetComponent<Collider>() != null)
                equipmentItem.GetComponent<Collider>().enabled = false;

            objectInHand = equipmentItem;
            objectInHand.action.UpdateIsInHand(true);
        }

        internal void DropHeldItem()
        {
            if (objectInHand == null) return;

            EquipmentItem equipmentItem = hotbarSlots[currentScrollValue - 1].heldItem;

            if (equipmentItem == null)
            {
                Debug.LogError("Tried to drop, but HoldItem is null");
                return;
            }

            hotbarSlots[currentScrollValue - 1].heldItem = null;
            hotbarSlots[currentScrollValue - 1].slot.DisableSlotIcons();

            equipmentItem.transform.parent = worldHeldItemParent;
            equipmentItem.isInInventory = false;

            if (equipmentItem.GetComponent<Rigidbody>() != null)
                equipmentItem.GetComponent<Rigidbody>().isKinematic = false;

            if (equipmentItem.GetComponent<Collider>() != null)
                equipmentItem.GetComponent<Collider>().enabled = true;

            objectInHand.action.UpdateIsInHand(false);
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
            drop.GetComponent<ResourceItem>().itemScriptable.interactText = itemSO.item.interactText;
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