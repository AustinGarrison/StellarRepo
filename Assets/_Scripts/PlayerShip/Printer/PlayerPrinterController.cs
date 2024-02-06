using Printer.Model;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;


// Handle
public class PlayerPrinterController : MonoBehaviour
{
    [SerializeField] private UIPrinterPage printerUI;
    [SerializeField] private SO_ShipPrinter printerData;

    public List<PrinterItem> initialItems = new List<PrinterItem>();

    private void Awake()
    {
        PrepareUI();
        PrepareCraftingData();
    }

    // Spawn in the empty bastards and assign pointer events
    private void PrepareUI()
    {
        // Add Empty Buttons
        printerUI.InitializePrinterUI(printerData.Size);
        printerUI.OnDescriptionRequested += HandleDescriptionRequest;
    }

    private void PrepareCraftingData()
    {
        printerData.Initialize();
        printerData.OnCraftingUpdated += UpdatePrinterUI;

        int index = 0;
        foreach (PrinterItem item in initialItems)
        {
            printerData.AddItem(index, item);
            index++;
        }

        printerData.InformAboutChange();
    }


    // Is called when a craftable item is clicked
    private void HandleDescriptionRequest(int itemIndex)
    {
        PrinterItem printerItem = printerData.GetItemAt(itemIndex);
        SO_PrinterIcon item = printerItem.item;
        printerUI.UpdateDescription(itemIndex, item.ItemImage, item.name, item.ItemDescription);
    }

    private void UpdatePrinterUI(Dictionary<int, PrinterItem> inventoryState)
    {
        // WHERE THE PROBLEM IS HAPPENING
        printerUI.ResetAllItems();
        //foreach (var item in inventoryState)
        //{
        //    printerUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.item.ItemName);
        //}
    }
}