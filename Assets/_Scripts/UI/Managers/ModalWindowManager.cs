using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ModalWindowManager : MonoBehaviour
    {
        // Resources
        public UIButtonManager confirmButton;
        public UIButtonManager cancelButton;
        [SerializeField] private Animator modalWindowAnimator;

        // Settings
        public bool isOn = false;
        public bool closeOnCancel = true;
        public bool closeOnConfirm = true;
        public bool showCancelButton = true;
        public bool showConfirmButton = true;
        [Range(0.5f, 2)] public float animationSpeed = 1;
        public StartBehaviour startBehaviour = StartBehaviour.Disable;
        public CloseBehaviour closeBehaviour = CloseBehaviour.Disable;
        public InputType inputType = InputType.Focused;

        // Events
        public UnityEvent onConfirm = new UnityEvent();
        public UnityEvent onCancel = new UnityEvent();
        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent onClose = new UnityEvent();

        // Helpers
        string animationIn = "In";
        string animationOut = "Out";
        string animationSpeedKey = "AnimSpeed";


        // Event System
        bool canProcessEventSystem;
        float openStateLength;
        float closeStateLength;
        GameObject latestEventSystemObject;

        public enum StartBehaviour { Enable, Disable }
        public enum CloseBehaviour { Disable, Destroy }
        public enum InputType { Focused, Free }

        private void Awake()
        {
            InitModalWindow();
            InitEventSystem();
            UpdateUI();
        }

        private void Start()
        {
            if (startBehaviour == StartBehaviour.Disable) { isOn = false; gameObject.SetActive(false); }
            else if(startBehaviour == StartBehaviour.Enable) { isOn = false; OpenWindow(); }
        }


        void InitModalWindow()
        {
            if (modalWindowAnimator == null) { modalWindowAnimator = gameObject.GetComponent<Animator>(); }
            if (closeOnCancel) { onCancel.AddListener(CloseWindow); }
            if (closeOnConfirm) { onConfirm.AddListener(CloseWindow); }
            if (confirmButton != null) { confirmButton.onClick.AddListener(onConfirm.Invoke); }
            if (cancelButton != null) { cancelButton.onClick.AddListener(onCancel.Invoke); }

            openStateLength = UIInternalTools.GetAnimatorClipLength(modalWindowAnimator, "ModalWindow_In");
            closeStateLength = UIInternalTools.GetAnimatorClipLength(modalWindowAnimator, "ModalWindow_Out");
        }

        void InitEventSystem()
        {
            if(cancelButton == null && confirmButton == null) { canProcessEventSystem = false; }
            else { canProcessEventSystem = true; }
        }


        public void UpdateUI()
        {

            if (showCancelButton && cancelButton != null) { cancelButton.gameObject.SetActive(true); }
            else if (cancelButton != null) { cancelButton.gameObject.SetActive(false); }

            if (showConfirmButton && confirmButton != null) { confirmButton.gameObject.SetActive(true); }
            else if (confirmButton != null) { confirmButton.gameObject.SetActive(false); }
        }

        public void OpenWindow()
        {
            if(isOn) { return; }
            if(EventSystem.current.currentSelectedGameObject != null) { latestEventSystemObject = EventSystem.current.currentSelectedGameObject; }

            gameObject.SetActive(true);
            isOn = true;

            StopCoroutine("DisableObject");
            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            modalWindowAnimator.enabled = true;
            modalWindowAnimator.SetFloat(animationSpeedKey, animationSpeed);
            modalWindowAnimator.Play(animationIn);
            onOpen.Invoke();
        }

        public void CloseWindow()
        {
            if (!isOn)
                return;

            if (gameObject.activeSelf == true)
            {
                StopCoroutine("DisableObject");
                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableObject");
            }

            isOn = false;
            modalWindowAnimator.enabled = true;
            modalWindowAnimator.SetFloat(animationSpeedKey, animationSpeed);
            modalWindowAnimator.Play(animationOut);
            onClose.Invoke();
        }

        public void AnimateWindow()
        {
            if (!isOn) { OpenWindow(); }
            else { CloseWindow(); }
        }

        IEnumerator DisableObject()
        {
            yield return new WaitForSeconds(closeStateLength);
            if(closeBehaviour == CloseBehaviour.Disable) { gameObject.SetActive(false); }
            else if(closeBehaviour == CloseBehaviour.Destroy) { Destroy(gameObject); }

            modalWindowAnimator.enabled = false;
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(openStateLength + 0.1f);
            modalWindowAnimator.enabled = false;
        }
    }
}