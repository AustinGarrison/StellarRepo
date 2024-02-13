using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UI.Audio;
using Mono.CSharp.Linq;
using UnityEditor;

namespace UI
{
    public class PanelButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        // Resources
        [SerializeField] private CanvasGroup disabledCanvasGroup;
        [SerializeField] private CanvasGroup normalCanvasGroup;
        [SerializeField] private CanvasGroup highlightedCanvasGroup;
        [SerializeField] private CanvasGroup selectCanvasGroup;
        [SerializeField] private GameObject seperator;

        // Settings
        public bool isInteractable = true;
        public bool isSelected;

        public bool useSeperator = true;

        public bool useSounds = true;
        [Range(1, 15)] public float fadingMultiplier = 8;


        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();
        public UnityEvent onSelect = new UnityEvent();

        // Helpers
        bool isInitialized = false;
        ButtonPrintTester targetButton;
        [HideInInspector] public NavigationBar navbar;

        private void OnEnable()
        {
            if(!isInitialized) { Initialize(); }
            UpdateUI();
        }

        void Initialize()
        {
            if (!Application.isPlaying) { return; }
            if(UIManagerAudio.instance == null) { useSounds = false; }

            if(gameObject.GetComponent<Image>() == null)
            {
                Image raycastImage = gameObject.AddComponent<Image>();
                raycastImage.color = new Color(0, 0, 0, 0);
                raycastImage.raycastTarget = true;
            }

            disabledCanvasGroup.alpha = 0;
            normalCanvasGroup.alpha = 1;
            highlightedCanvasGroup.alpha = 0;
            selectCanvasGroup.alpha = 0;

            isInitialized = true;
        }

        public void IsInteractable(bool value)
        {
            isInteractable = value;

            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable && !isSelected) { StartCoroutine("SetNormal"); }
        }

        public void UpdateUI()
        {
            if(useSeperator && transform.parent != null && transform.GetSiblingIndex() != transform.parent.childCount - 1 && seperator != null) { seperator.SetActive(true); }
            else if(seperator != null) { seperator.SetActive(false); }

            if(isSelected)
            {
                disabledCanvasGroup.alpha = 0;
                normalCanvasGroup.alpha = 0;
                highlightedCanvasGroup.alpha = 0;
                selectCanvasGroup.alpha = 1;
            }
            else if (!isInteractable)
            {
                disabledCanvasGroup.alpha = 1;
                normalCanvasGroup.alpha = 0;
                highlightedCanvasGroup.alpha = 0;
                selectCanvasGroup.alpha = 0;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
            if (navbar != null) { navbar.LitButtons(this); }
            if (isSelected) { StartCoroutine("SetSelect"); onSelect.Invoke(); }
            else { StartCoroutine("SetNormal"); }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable) { return; }
            if (useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.clickSound); }

            onClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (navbar != null) { navbar.DimButtons(this); }
            if (useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.hoverSound); }
            if (!isInteractable || isSelected) { return; }

            onHover.Invoke();
            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (navbar != null) { navbar.LitButtons(); }
            if (!isInteractable || isSelected) { return; }

            onLeave.Invoke();
            StartCoroutine("SetNormal");
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetHighlight");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            StartCoroutine("SetNormal");
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isInteractable || isSelected)
                return;

            onClick.Invoke();
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetHighlight");
            StopCoroutine("SetSelect");

            while (normalCanvasGroup.alpha < 0.99f)
            {
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                highlightedCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCanvasGroup.alpha = 0;
            normalCanvasGroup.alpha = 1;
            highlightedCanvasGroup.alpha = 0;
            selectCanvasGroup.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetNormal");
            StopCoroutine("SetSelect");

            while (highlightedCanvasGroup.alpha < 0.99f)
            {
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightedCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                selectCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCanvasGroup.alpha = 0;
            normalCanvasGroup.alpha = 0;
            highlightedCanvasGroup.alpha = 1;
            selectCanvasGroup.alpha = 0;
        }

        IEnumerator SetSelect()
        {
            StopCoroutine("SetDisabled");
            StopCoroutine("SetNormal");
            StopCoroutine("SetHighlight");

            while (selectCanvasGroup.alpha < 0.99f)
            {
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightedCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                selectCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplier;
                yield return null;
            }

            disabledCanvasGroup.alpha = 0;
            normalCanvasGroup.alpha = 0;
            highlightedCanvasGroup.alpha = 0;
            selectCanvasGroup.alpha = 1;
        }
    }
}