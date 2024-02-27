using CallSOS.Printer.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CallSOS.Printer.UI
{
    public class ShipPrinterViewDescription : MonoBehaviour
    {
        [SerializeField] private RectTransform descriptionPanel;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;

        internal void Initialize()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            descriptionPanel.gameObject.SetActive(false);
            //itemImage.sprite = null;
            //title.text = "";
            //description.text = "";
            //gameObject.SetActive(false);
        }

        public void UpdateDescription(SO_PrinterIcon item)
        {
            descriptionPanel.gameObject.SetActive(true);
            itemImage.sprite = item.ItemImage;
            this.title.text = item.ItemName + " - " + item.ItemDescription;
        }
    }
}