using System;
using System.Collections.Generic;
using UnityEngine;

namespace Printer.Model
{
    // Handles all data logic
    // Interact with database
    // Never handle user requests/what to do with failure and success

    [CreateAssetMenu(fileName = "Printer", menuName = "ScriptableObject/PlayerShip/3DPrinter/PrinterModel")]
    public class SO_ShipPrinterModel : ScriptableObject
    {
        [SerializeField] private List<ShipPrinterCategory> initialCategories;

        [SerializeField] internal List<ShipPrinterCategory> currentCategories;

        internal void Initialize()
        {
            currentCategories = new List<ShipPrinterCategory>();
            for (int i = 0; i < initialCategories.Count; i++)
            {
                ShipPrinterCategory newCategory = initialCategories[i];
                newCategory.itemIndex = i;
                newCategory.icon.itemType = newCategory.itemType;
                currentCategories.Add(newCategory);
            }
        }

        // Returns a copy of currentCategories. Cant give data to view
        internal List<ShipPrinterCategory> RequestIconInfo()
        {
            List<ShipPrinterCategory> categoryList = new List<ShipPrinterCategory>();

            foreach (ShipPrinterCategory category in currentCategories)
            {
                categoryList.Add(category);
            }

            return categoryList;
        }

        public ShipPrinterCategory GetItemAt(int index)
        {
            return currentCategories[index];
        }

    }

    [Serializable]
    public struct ShipPrinterCategory
    {
        public SO_PrinterIcon icon;
        public ShipPrinterItemType itemType;
        [HideInInspector] public int itemIndex;
    }

    public enum ShipPrinterItemType
    {
        Category,
        Craftable
    }
}