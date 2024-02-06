using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrinterItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image iconBorder;

    public event Action<UIPrinterItem> OnItemClicked, OnRightMouseBtnClick;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        Deselect();
        Debug.Log("AmI created Yey");
    }


    public void Select()
    {
        iconBorder.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        iconBorder.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, string name)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        itemName.text = name;
    }

    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }
}
