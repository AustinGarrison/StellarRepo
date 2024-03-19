using CallSOS.Player.Interaction.Equipment;
using System.Collections.Generic;
using System.Collections;
using CallSOS.Utilities;
using FishNet.Object;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using FishNet;

namespace CallSOS.Player.Interaction
{
    public enum EquipmentItemHandType
    {
        OneHanded,
        TwoHanded
    }

    public class InventoryController : NetworkBehaviour
    {
        [SerializeField] internal Transform worldHeldItemParent;
        [SerializeField] private ObjectInteractController interactController;

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
        private const int minScrollValue = 0;
        private const int maxScrollValue = 4;
        public int currentScrollValue = 0;

        // Helpers
        bool isInCooldown = false;
        bool isInitialized = false;
        int twoHandedSlot = 4;

        #region EventArgs

        public event EventHandler<GetEquipmentEventArgs> OnGetEquipment;

        public class GetEquipmentEventArgs : EventArgs
        {
            public EquipmentItem EquipmentItem { get; }

            public GetEquipmentEventArgs(EquipmentItem item)
            {
                EquipmentItem = item;
            }
        }

        private void GameInput_OnResourceHUDToggled(object sender, EventArgs e)
        {
            ToggleResourceUI();
        }

        private void GameInput_OnAltInteractAction(object sender, EventArgs e)
        {
            DropHeldItem();
        }

        private void InteractController_OnHoldItemInteract(object sender, ObjectInteractController.EquipmentItemEventArgs e)
        {
            FindEmptyHotbarSlot(e.EquipmentItem);
        }

        private void InteractController_OnResourceItemInteract(object sender, ObjectInteractController.ResourceItemEventArgs e)
        {
            AddResource(e.ResourceItem);
        }

        #endregion

        internal void Initialize()
        {
            //Turns off inventory if its on
            if (invPanel != null && invPanel.activeSelf)
                ToggleResourceUI();

            //Toggle Inventory Screen
            GameInputPlayer.Instance.OnResourceHUDToggled += GameInput_OnResourceHUDToggled;

            //Drop held item
            GameInputPlayer.Instance.OnAltInteractAction += GameInput_OnAltInteractAction;

            //Pickup equipment
            interactController.OnEquipmentItemInteract += InteractController_OnHoldItemInteract;

            //Pickup Resource
            interactController.OnResourceItemInteract += InteractController_OnResourceItemInteract;

            isInitialized = true;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            GameInputPlayer.Instance.OnResourceHUDToggled -= GameInput_OnResourceHUDToggled;
            GameInputPlayer.Instance.OnAltInteractAction -= GameInput_OnAltInteractAction;

            interactController.OnEquipmentItemInteract -= InteractController_OnHoldItemInteract;
            interactController.OnResourceItemInteract -= InteractController_OnResourceItemInteract;
        }

        private void Update()
        {
            if (!isInitialized) return;
            GetScrollValue();
        }

        #region Equipment
        private void GetScrollValue()
        {
            if (isInCooldown) return;

            float scrollInput = GameInputPlayer.Instance.GetScrollAxis() / 1000;

            if (scrollInput == 0) return;

            int previousScrollValue = currentScrollValue;

            if (scrollInput < 0)
            {
                currentScrollValue = (currentScrollValue + 1) % (maxScrollValue + 1);
            }
            else if (scrollInput > 0)
            {
                currentScrollValue = (currentScrollValue - 1 + maxScrollValue + 1) % (maxScrollValue + 1);
            }

            ChangeActiveHotbarSlot(previousScrollValue);
        }

        private void SetActiveItem()
        {
            if(objectInHand != null)
                objectInHand.action.UpdateIsInHand(false);

            if (hotbarSlots[currentScrollValue].heldItem != null)
            {
                objectInHand = hotbarSlots[currentScrollValue].heldItem;
                objectInHand.action.UpdateIsInHand(true);

                OnGetEquipment?.Invoke(this, new GetEquipmentEventArgs(objectInHand));
            }
                
            if (hotbarSlots[currentScrollValue].heldItem == null)
            {
                objectInHand = null;
            }   
        }

        private void UpdateUISlotHighlights(int currentScrollValue, int previousScrollValue)
        {
            if (hotbarSlots[currentScrollValue].slot == null)
            {
                Debug.LogWarning("Hotbar reference is empty. Check Inspector value");
            }

            if (previousScrollValue != -1) hotbarSlots[previousScrollValue].slot.StartCoroutine("SetNormal");
            hotbarSlots[currentScrollValue].slot.StartCoroutine("SetHighlight");
        }

        /// <summary>
        /// Searches the players hotbar array for an empty slot to put
        /// the picked up item
        /// </summary>
        internal void FindEmptyHotbarSlot(EquipmentItem item)
        {
            if (holdPostion == null)
            {
                Debug.LogError("No hold position found");
                return;
            }

            // Item clicked is a One handed item
            if (item.handType == EquipmentItemHandType.OneHanded)
            {
                // Currently Selected slot is open, and not two handed slot
                if (hotbarSlots[currentScrollValue].heldItem == null && currentScrollValue != twoHandedSlot)
                {
                    int hotbarSlot = currentScrollValue;

                    AddItemToFoundSlot(hotbarSlot, item);
                }
                else
                {
                    bool hasFoundSlot = false;

                    // Loop through all slots to find first open
                    hasFoundSlot = FindFirstSlot(item, hasFoundSlot);

                    // No Slot Found
                    if (!hasFoundSlot)
                    {
                        Debug.Log("No Slot Found");

                        return;
                    }
                }
            }

            // Item clicked is a two handed item
            if (item.handType == EquipmentItemHandType.TwoHanded)
            {
                if (hotbarSlots[twoHandedSlot].heldItem == null)
                {
                    int hotbarSlot = twoHandedSlot;

                    AddItemToFoundSlot(hotbarSlot, item);
                }
                else
                {
                    Debug.Log("Two Handed Is Full");
                }
            }
        }

        /// <summary>
        /// Loop through all slots to find an unoccupied slot
        /// </summary>
        private bool FindFirstSlot(EquipmentItem item, bool hasFoundSlot)
        {
            for (int index = 0; index < hotbarSlots.Length; index++)
            {
                // Currently Selected slot is open
                if (hotbarSlots[index].heldItem == null)
                {
                    int hotbarSlot = index;

                    AddItemToFoundSlot(hotbarSlot, item);

                    hasFoundSlot = true;

                    // finished with search loop
                    break;
                }
            }

            return hasFoundSlot;
        }

        /// <summary>
        /// When attempting to pickup an item, this is called
        /// if an empty slot is found
        /// </summary>
        private void AddItemToFoundSlot(int hotbarSlot, EquipmentItem item)
        {
            hotbarSlots[hotbarSlot].heldItem = item;
            hotbarSlots[hotbarSlot].slot.UpdateSlotIcons(item.iconImage);

            int previousScrollValue = currentScrollValue;
            currentScrollValue = hotbarSlot;

            ChangeActiveHotbarSlot(previousScrollValue);

            UpdateUISlotHighlights(hotbarSlot, previousScrollValue);

            PickupHeldItem(item);
        }

        /// <summary>
        /// Changes which hotbar slot is currently selected
        /// </summary>
        private void ChangeActiveHotbarSlot(int previousScrollValue)
        {
            SetActiveItem();
            UpdateUISlotHighlights(currentScrollValue, previousScrollValue);

            // Disable all hotbar equipment, before enabling the held one
            foreach (var item in hotbarSlots)
            {
                if (item.heldItem != null)
                {
                    item.heldItem.gameObject.SetActive(false);

                    OnGetEquipment?.Invoke(this, new GetEquipmentEventArgs(objectInHand));
                }
            }

            // Enable the current equipment
            if (hotbarSlots[currentScrollValue].heldItem != null)
                hotbarSlots[currentScrollValue].heldItem.gameObject.SetActive(true);

            // Set scrolling cooldown
            isInCooldown = true;
            StopCoroutine("ScrollCooldownTimer");
            StartCoroutine("ScrollCooldownTimer");
        }

        /// <summary>
        /// Move item from world to players hand
        /// </summary>
        private void PickupHeldItem(EquipmentItem equipmentItem)
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

        /// <summary>
        /// Remove held item from hotbar and set it back to the world
        /// </summary>
        internal void DropHeldItem()
        {
            if (objectInHand == null) return;

            EquipmentItem equipmentItem = hotbarSlots[currentScrollValue].heldItem;

            if (equipmentItem == null)
            {
                Debug.LogError("Tried to drop, but HoldItem is null");
                return;
            }

            hotbarSlots[currentScrollValue].heldItem = null;
            hotbarSlots[currentScrollValue].slot.DisableSlotIcons();

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
        #endregion
        #region Resources


        internal void UpdateResourceList()
        {
            foreach (Transform child in UIinvObjectHolder)
                Destroy(child.gameObject);

            foreach (ResourceObject inventoryObject in resourceObjects)
            {
                GameObject obj = Instantiate(UIInvObjectPrefab, UIinvObjectHolder);
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventoryObject.item.itemName + " - " + inventoryObject.amount;
                obj.GetComponent<Button>().onClick.AddListener(delegate { DropOneResourceItem(inventoryObject.item); });
            }
        }

        internal void ToggleResourceUI()
        {
            if(invPanel == null)
            {
                Debug.LogWarning("invPanel not found");
                return;
            }

            if (invPanel.activeSelf)
            {
                invPanel.SetActive(false);
                //crosshair.SetActive(true);
                //Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
            }
            else if (!invPanel.activeSelf)
            {

                UpdateResourceList();
                invPanel.SetActive(true);
                //crosshair.SetActive(false);
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
            }
        }

        internal void AddResource(ResourceItem newItem)
        {
            foreach (ResourceObject inventoryObject in resourceObjects)
            {
                if (inventoryObject.item == newItem.itemScriptable)
                {
                    inventoryObject.amount++;
                    inventoryObject.isNetworked = newItem.isNetworked;
                    return;
                }
            }

            resourceObjects.Add(new ResourceObject() { item = newItem.itemScriptable, amount = 1, isNetworked = newItem.isNetworked});
        }

        void DropOneResourceItem(ItemSO item)
        {
            foreach (ResourceObject resourceObject in resourceObjects)
            {
                if (resourceObject.item != item)
                    continue;

                // If there is more than one item, only drop one from the list
                if (resourceObject.amount > 1)
                {
                    resourceObject.amount--;
                    DropResource(resourceObject, transform.position + transform.forward);
                    UpdateResourceList();
                    return;
                }

                // If there is only one item left, remove it from the inventory list.
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
            if (itemSO.isNetworked)
            {
                DropResourceServerRpc(itemSO, position);
            }
            else
            {
                GameObject drop = Instantiate(itemSO.item.prefab, position, Quaternion.identity, worldHeldItemParent);
                drop.name = itemSO.item.name;

                ResourceItem resourceItem = drop.GetComponent<ResourceItem>();

                if (resourceItem != null)
                {
                    resourceItem.itemScriptable.interactText = itemSO.item.interactText;
                }
            }
        }


        [ServerRpc]
        void DropResourceServerRpc(ResourceObject itemSO, Vector3 position)
        {
            GameObject drop = Instantiate(itemSO.item.prefab, position, Quaternion.identity);

            ServerManager.Spawn(drop);


            ResourceItem resourceItem = drop.GetComponent<ResourceItem>();

            if (resourceItem != null)
            {
                resourceItem.itemScriptable.interactText = itemSO.item.interactText;
            }


            drop.name = "P9ink:";
        }

#endregion
    }

    [System.Serializable]
    public class ResourceObject
    {
        public ItemSO item;
        public int amount;
        public bool isNetworked;
    }
}