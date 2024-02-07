using Printer.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Printer.UI
{
    public class ShipPrinterViewCategories : MonoBehaviour
    {
        [SerializeField] private ShipPrinterIcon itemPrefab;
        [SerializeField] private RectTransform categoryPanel;

        private List<ShipPrinterIcon> itemIconsList;
        public event Action<int> OnPrinterIconLeftClicked;

        private void Awake()
        {
            itemIconsList = new List<ShipPrinterIcon>();
        }

        internal void Initialize(int numOfCategories, List<ShipPrinterCategory> initialPrinterCategories)
        {
            for (int i = 0; i < numOfCategories; i++)
            {
                ShipPrinterCategory category = initialPrinterCategories[i];

                ShipPrinterIcon uiIcon =
                    Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);

                uiIcon.transform.SetParent(categoryPanel, false);

                uiIcon.SetData(category.icon.ItemImage, category.icon.ItemName, category.itemIndex);

                uiIcon.OnItemRightClicked += UiIcon_OnRightMouseBtnClick;
                uiIcon.OnItemLeftClicked += UiIcon_OnItemLeftClicked;
                
                itemIconsList.Add(uiIcon);
            }
        }

        private void UiIcon_OnItemLeftClicked(ShipPrinterIcon printerIcon)
        {
            int index = itemIconsList.IndexOf(printerIcon);

            if (index == -1)
                return;

            DeselectAllItems();
            HighlightSelected(index);
            OnPrinterIconLeftClicked?.Invoke(index);
        }

        private void UiIcon_OnRightMouseBtnClick(ShipPrinterIcon printerIcon)
        {
            throw new NotImplementedException();
        }

        private void DeselectAllItems()
        {
            foreach (ShipPrinterIcon item in itemIconsList)
            {
                item.Deselect();
            }
        }

        private void HighlightSelected(int index)
        {
            itemIconsList[index].Select();
        }

        public void ClearIcons()
        {
            foreach(ShipPrinterIcon obj in itemIconsList)
            {
                Destroy(obj.gameObject);
            }

            itemIconsList.Clear();
        }

    }

}