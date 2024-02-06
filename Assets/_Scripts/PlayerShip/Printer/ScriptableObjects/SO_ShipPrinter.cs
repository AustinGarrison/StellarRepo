using System;
using UnityEngine;
using System.Collections.Generic;

namespace Printer.Model
{
    [CreateAssetMenu(fileName = "Printer", menuName = "ScriptableObject/PlayerShip/3DPrinter/Printer")]
    public class SO_ShipPrinter : ScriptableObject
    {
        [SerializeField] private List<PrinterItem> printerItems;
        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, PrinterItem>> OnCraftingUpdated;
        
        public void Initialize()
        {
            printerItems = new List<PrinterItem>();
            for(int i = 0; i < Size; i++)
            {
                printerItems.Add(PrinterItem.GetEmptyCategory());
            }
        }

        public void AddItem(int itemIndex, PrinterItem item)
        {
            PrinterItem newItem = new PrinterItem
            {
                item = item.item
            };

            printerItems[itemIndex] = newItem;
        }


        public PrinterItem GetItemAt(int itemIndex)
        {
            return printerItems[itemIndex];
        }

        internal void InformAboutChange()
        {
            OnCraftingUpdated?.Invoke(GetCurrentCraftingState());
        }

        public List<PrinterItem> GetPrinterItems()
        {
            List<PrinterItem> printerItemsRef = new List<PrinterItem>();

            for (int i = 0; i < printerItems.Count; i++)
            {

                printerItemsRef.Add(printerItems[i]);
            }

            return printerItemsRef;
        }

        public Dictionary<int, PrinterItem> GetCurrentCraftingState()
        {
            Dictionary<int, PrinterItem> returnValue = 
                new Dictionary<int, PrinterItem>();

            for (int i = 0; i < printerItems.Count; i++)
            {
                if (printerItems[i].IsEmpty)
                {
                    continue;
                }
                    
                returnValue[i] = printerItems[i];
            }

            return returnValue;
        }
    }

    [Serializable]
    public struct PrinterItem
    {
        public SO_PrinterIcon item;

        public bool IsEmpty => item == null;

        public static PrinterItem GetEmptyCategory()
            => new PrinterItem
            {
                item = null
            };
    }
}