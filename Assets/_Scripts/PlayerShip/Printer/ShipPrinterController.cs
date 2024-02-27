using CallSOS.Printer.Model;
using CallSOS.Printer.UI;
using UnityEngine;

namespace CallSOS.Printer.Controller
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
            printerCategoryUI.Initialize(printerData.currentCategories.Count, printerData.RequestIconInfo());

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
            printerDescriptionUI.UpdateDescription(item);
        }

        private void CategoryClicked(ShipPrinterCategory printerCategory)
        {
            printerCategoryUI.ClearIcons();
            printerData.NewCategories(printerCategory);
            printerCategoryUI.Initialize(printerData.currentCategories.Count, printerData.RequestIconInfo());
        }

        public void ReloadInitial()
        {
            printerDescriptionUI.ResetDescription();

            // Delete Whats Current
            printerCategoryUI.ClearIcons();

            // Initialize Data
            printerData.Initialize();

            // Initialize UI
            printerCategoryUI.Initialize(printerData.currentCategories.Count, printerData.RequestIconInfo());
        }
    }
}