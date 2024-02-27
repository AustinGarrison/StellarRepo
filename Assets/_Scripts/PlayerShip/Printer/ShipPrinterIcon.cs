using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CallSOS.Printer.UI
{
    public class ShipPrinterIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text iconText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image iconBorder;
        internal int itemIndex;

        public event Action<ShipPrinterIcon> OnItemLeftClicked, OnItemRightClicked;

        private void Awake()
        {
            iconImage.gameObject.SetActive(false);
            Deselect();
        }

        internal void Select()
        {
            iconBorder.gameObject.SetActive(true);
        }

        internal void Deselect()
        {
            iconBorder.gameObject.SetActive(false);
        }

        public void SetData(Sprite image, string text, int itemIndex)
        {
            iconImage.sprite = image;
            iconText.text = text;
            this.itemIndex = itemIndex;
            iconImage.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                OnItemRightClicked?.Invoke(this);
            }
            else if(eventData.button == PointerEventData.InputButton.Left)
            {
                OnItemLeftClicked?.Invoke(this);
            }
        }
    }
}