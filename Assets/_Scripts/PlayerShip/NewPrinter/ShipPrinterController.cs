using Printer.Model;
using Printer.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Printer
{
    // Handle requests from client
    // Contains Little Code

    // Asks model for information
    // Never interact with data
    public class ShipPrinterController : MonoBehaviour
    {
        [SerializeField] private ShipPrinterViewCategories printerCategoryUI;
        [SerializeField] private ShipPrinterViewDescription printerDescriptionUI;
        [SerializeField] private SO_ShipPrinterModel printerData;

        // Initialize
        private void Awake()
        {
            // Initialize Data
            printerData.Initialize();

            // Initialize UI
            List<ShipPrinterIcon> itemIcons 
                = printerCategoryUI.Initialize(printerData.currentCategories.Count, printerData.RequestIconInfo());
            printerCategoryUI.OnPrinterIconLeftClicked += CategoryUI_OnIconLeftClick;

            printerDescriptionUI.Initialize();
        }

        private void CategoryUI_OnIconLeftClick(int itemIndex)
        {
            ShipPrinterCategory printerIcon = printerData.GetItemAt(itemIndex);

            if (printerIcon.itemType == ShipPrinterItemType.Craftable)
            {
                PrintableClicked(printerIcon);
            }
            else if(printerIcon.itemType == ShipPrinterItemType.Category)
            {
                CategoryClicked(printerIcon);
            }
        }

        private void PrintableClicked(ShipPrinterCategory printerCraftable)
        {
            SO_PrinterIcon item = printerCraftable.icon;
            printerDescriptionUI.UpdateDescription(item.ItemImage, item.ItemName, item.ItemDescription);
        }

        private void CategoryClicked(ShipPrinterCategory printerCategory)
        {

        }
    }
}