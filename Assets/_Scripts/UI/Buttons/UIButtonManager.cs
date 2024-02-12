using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UI.Audio;
using System.Data.Common;

namespace UI
{
    public class UIButtonManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {

        // Resources
        [SerializeField] private CanvasGroup normalCanvasGroup;
        [SerializeField] private CanvasGroup highlightCanvasGroup;
        [SerializeField] private CanvasGroup disabledCanvasGroup;

        // Auto Size
        public bool autoFitContent = true;

        // Settings
        public bool isInteractable = true;

        public bool checkForDoubleClick = true;

        public bool bypassUpdateOnEnable = false;

        public bool useSounds = true;
        [Range(0.1f, 1)] public float doubleClickPeriod = 0.25f;
        [Range(1, 15)] public float fadingMultiplier = 8;

        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDoubleClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();

        // Helpers
        bool isInitialized = false;

        bool waitingForDoubleClickInput;

        void OnEnable()
        {
            if (!isInitialized) { Initialize(); }
            if (!bypassUpdateOnEnable) UpdateUI();
        }

        void OnDisable()
        {
            if (!isInitialized)
                return;

            if (disabledCanvasGroup != null) { disabledCanvasGroup.alpha = 0; }
            if (normalCanvasGroup != null) { normalCanvasGroup.alpha = 1; }
            if (highlightCanvasGroup != null) { highlightCanvasGroup.alpha = 0; }
        }

        void Initialize()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying) { return; }
#endif
            //if(ControllerManager.instance != null)
            //{
            //  ControllerManager.instance.buttons.Add(this);
            //}

            if (UIManagerAudio.instance == null)
            {
                useSounds = false;
            }

            if (normalCanvasGroup == null)
            { 
                normalCanvasGroup = new GameObject().AddComponent<CanvasGroup>();
                normalCanvasGroup.gameObject.AddComponent<RectTransform>();
                normalCanvasGroup.transform.SetParent(transform);
                normalCanvasGroup.gameObject.name = "Normal";
            }

            if (highlightCanvasGroup == null)
            {
                highlightCanvasGroup = new GameObject().AddComponent<CanvasGroup>();
                highlightCanvasGroup.gameObject.AddComponent<RectTransform>();
                highlightCanvasGroup.transform.SetParent(transform);
                highlightCanvasGroup.gameObject.name = "Highlight";
            }

            if (disabledCanvasGroup == null)
            {
                disabledCanvasGroup = new GameObject().AddComponent<CanvasGroup>();
                disabledCanvasGroup.gameObject.AddComponent<RectTransform>();
                disabledCanvasGroup.transform.SetParent(transform);
                disabledCanvasGroup.gameObject.name = "Disabled";
            }

            if (GetComponent<Image>() == null)
            {
                Image raycastImg = gameObject.AddComponent<Image>();
                raycastImg.color = new Color(0, 0, 0, 0);
                raycastImg.raycastTarget = true;
            }

            normalCanvasGroup.alpha = 1;
            highlightCanvasGroup.alpha = 0;
            disabledCanvasGroup.alpha = 0;

            isInitialized = true;
        }

        public void UpdateUI()
        {

#if UNITY_EDITOR
            if (!Application.isPlaying && autoFitContent)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                if (disabledCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCanvasGroup.GetComponent<RectTransform>()); }
                if (normalCanvasGroup  != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>()); }
                if (highlightCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCanvasGroup.GetComponent<RectTransform>()); }
            }
#endif

            if (!Application.isPlaying || !gameObject.activeInHierarchy) { return; }
            if (!isInteractable) { StartCoroutine("SetDisabled"); }
            else if (isInteractable && disabledCanvasGroup.alpha == 1) { StartCoroutine("SetNormal"); }

            StartCoroutine("LayoutFix");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!isInitialized || eventData.button != PointerEventData.InputButton.Left) { return; }
            if(useSounds) { UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.clickSound); }

            onClick.Invoke();

            // Check for double click
            if(!checkForDoubleClick) { return; }
            if (waitingForDoubleClickInput)
            {
                onDoubleClick.Invoke();
                waitingForDoubleClickInput = false;
                return;
            }
            waitingForDoubleClickInput = true;

            if(gameObject.activeInHierarchy)
            {
                StopCoroutine("CheckForDoubleClick");
                StartCoroutine("CheckForDoubleClick");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!isInteractable)
            {
                return;
            }

            if(useSounds)
            {
                UIManagerAudio.instance.audioSource.PlayOneShot(UIManagerAudio.instance.UIManagerAsset.hoverSound);
            }

            StartCoroutine("SetHighlight");
            onHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable)
                return;

            StartCoroutine("SetNormal");
            onLeave.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator LayoutFix()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

            if (disabledCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(disabledCanvasGroup.GetComponent<RectTransform>()); }
            if (normalCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(normalCanvasGroup.GetComponent<RectTransform>()); }
            if (highlightCanvasGroup != null) { LayoutRebuilder.ForceRebuildLayoutImmediate(highlightCanvasGroup.GetComponent<RectTransform>()); }
        }

        IEnumerator SetNormal()
        {
            StopCoroutine("SetHighlight");
            StopCoroutine("SetDisabled");

            while(normalCanvasGroup.alpha < 0.99f)
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

            while (disabledCanvasGroup.alpha < 0.99)
            {
                normalCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;
                disabledCanvasGroup.alpha -= Time.unscaledDeltaTime * fadingMultiplier;

                yield return null;
            }

            normalCanvasGroup.alpha = 0;
            highlightCanvasGroup.alpha = 0;
            disabledCanvasGroup.alpha = 1;
        }

        IEnumerator CheckForDoubleClick()
        {
            yield return new WaitForSecondsRealtime(doubleClickPeriod);
            waitingForDoubleClickInput = false;
        }
    }
}