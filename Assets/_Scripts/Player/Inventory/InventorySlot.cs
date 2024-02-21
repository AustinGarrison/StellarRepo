using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Interaction
{

    public class InventorySlot : MonoBehaviour
    {
        // Resources
        [SerializeField] private CanvasGroup normalCanvasGroup;
        [SerializeField] private CanvasGroup highlightCanvasGroup;
        [SerializeField] private CanvasGroup disabledCanvasGroup;
        [SerializeField] private Image[] invItemIcons;

        // Settings
        [Range(1, 15)] public float fadingMultiplier = 8;
        public HoldItemHandType slotType;

        internal void UpdateSlotIcons(Sprite iconImage)
        {
            foreach (var item in invItemIcons)
            {
                item.sprite = iconImage;
                item.enabled = true;
            }
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");
            StopCoroutine("SetDisabled");

            while (normalCanvasGroup.alpha < 0.99f)
            {
                normalCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCanvasGroup.alpha = 1;
            highlightCanvasGroup.alpha = 0;
            disabledCanvasGroup.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetDisabled");

            while (highlightCanvasGroup.alpha < 0.99f)
            {
                normalCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCanvasGroup.alpha = 0;
            highlightCanvasGroup.alpha = 1;
            disabledCanvasGroup.alpha = 0;
        }

        IEnumerator SetDisabled()
        {
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (disabledCanvasGroup.alpha < 0.99f)
            {
                normalCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            normalCanvasGroup.alpha = 0;
            highlightCanvasGroup.alpha = 0;
            disabledCanvasGroup.alpha = 1;
        }
    }
}