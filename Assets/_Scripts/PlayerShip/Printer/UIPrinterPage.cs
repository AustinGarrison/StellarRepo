using System;
using System.Collections.Generic;
using UnityEngine;

// Handle Updating visuals only
public class UIPrinterPage : MonoBehaviour
{
    [SerializeField] private UIPrinterItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private UICraftingDescription craftDescription;

    [SerializeField] List<UIPrinterItem> listOfPrinterItems = new List<UIPrinterItem>();

    public event Action<int> OnDescriptionRequested;

    // Disable the printable item description panel
    private void Start()
    {
        craftDescription.ResetDescription();
        craftDescription.gameObject.SetActive(false);
    }

    public void InitializePrinterUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIPrinterItem uiItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel, false);
            listOfPrinterItems.Add(uiItem);
            uiItem.OnItemClicked += HandleCraftingItemClicked;
        }
    }

    private void HandleCraftingItemClicked(UIPrinterItem printerItem)
    {
        int index = listOfPrinterItems.IndexOf(printerItem);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
    }

    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
    {
        craftDescription.SetDescription(itemImage, name, description);
        craftDescription.gameObject.SetActive(true);
        DeselectAllItems();
        listOfPrinterItems[itemIndex].Select();
    }

    private void DeselectAllItems()
    {
        foreach (UIPrinterItem item in listOfPrinterItems)
        {
            item.Deselect();
        }
    }

    internal void ResetAllItems()
    {
        foreach (UIPrinterItem item in listOfPrinterItems)
        {
            item.ResetData();
            item.Deselect();
        }
    }

    public void PrintList()
    {
        foreach (var item in listOfPrinterItems)
        {
            Debug.Log(item);
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, string itemName)
    {
        if (listOfPrinterItems.Count > itemIndex)
        {
            listOfPrinterItems[itemIndex].SetData(itemImage, itemName);
        }
    }

}
