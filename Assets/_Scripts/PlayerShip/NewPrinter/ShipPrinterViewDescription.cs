using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Printer.UI
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

        public void UpdateDescription(Sprite image, string title, string description)
        {
            descriptionPanel.gameObject.SetActive(true);
            itemImage.sprite = image;
            this.title.text = title + " - " + description;
        }
    }
}