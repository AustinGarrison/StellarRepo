using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UI.Audio;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class HotkeyEvent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        // Content
        public HotkeyType hotkeyType = HotkeyType.Custom;
        public InputAction hotkey;
        public string hotkeyLabel = "Exit";

        // Resources
        [SerializeField] private CanvasGroup normalCanvasGroup;
        [SerializeField] private CanvasGroup highlightCanvasGroup;

        // Settings
        public bool useSounds = false;
        [Range(1, 15)] public float fadingMultiplayer = 8;

        // Events
        public UnityEvent onHotKeyPress = new UnityEvent();

        // Helpers
        bool isInitialized = false;

        public enum HotkeyType { Dynamic, Custom }

        void OnEnable()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying) { UpdateVisual(); }
            if(!Application.isPlaying) { return; }
#endif
            if(!isInitialized) { Initialize(); }

            UpdateUI();
        }

        void Update()
        {
            if(hotkey.triggered)
            {
                onHotKeyPress.Invoke();
            }
        }

        void Initialize()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying) { return; };
#endif
            hotkey.Enable();

            if(hotkeyType == HotkeyType.Dynamic && gameObject.GetComponent<Image>() == null)
            {
                Image raycastImage = gameObject.AddComponent<Image>();
                raycastImage.color = new Color(0, 0, 0, 0);
                raycastImage.raycastTarget = true;
            }

            if(UIManagerAudio.instance == null) { useSounds = false; }

            if(useSounds)
            {
                onHotKeyPress.AddListener(delegate { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.clickSound); });
            }

            isInitialized = true;
        }

        public void UpdateUI()
        {

            if (gameObject.activeInHierarchy == true && Application.isPlaying) { StartCoroutine("LayoutFix"); }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                if (normalCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>()); }
            }
        }

#if UNITY_EDITOR
        public void UpdateVisual()
        {
            if(highlightCanvasGroup != null) { highlightCanvasGroup.alpha = 0f; }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            if(normalCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>()); }
        }
#endif

        public void OnPointerClick(PointerEventData eventData)
        {
            onHotKeyPress.Invoke();

            if(gameObject.activeInHierarchy == true) { StartCoroutine("LayoutFix"); }
            else
            {
                if (normalCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>()); }
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.hoverSound); }
            if(normalCanvasGroup == null || highlightCanvasGroup == null)
                return;

            StopCoroutine("SetNormal");
            StartCoroutine("SetHighlight");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (normalCanvasGroup == null || highlightCanvasGroup == null)
                return;

            StopCoroutine("SetHighlight");
            StartCoroutine("SetNormal");
        }

        IEnumerator LayoutFix()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            if (normalCanvasGroup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>());
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }

        IEnumerator SetNormal()
        {
            while(highlightCanvasGroup.alpha > 0.01f)
            {
                highlightCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplayer;
                yield return null;
            }

            highlightCanvasGroup.alpha = 0;
        }

        IEnumerator SetHighlight()
        {
            while (highlightCanvasGroup.alpha < 0.01f)
            {
                highlightCanvasGroup.alpha += Time.unscaledDeltaTime * fadingMultiplayer;
                yield return null;
            }

            highlightCanvasGroup.alpha = 1;
        }
    }
}