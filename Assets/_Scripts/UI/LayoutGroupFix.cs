using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class LayoutGroupFix : MonoBehaviour
    {
        [SerializeField] private bool fixOnEnable = true;
        [SerializeField] private bool fixWithDelay = true;
        [SerializeField] private RebuildMethod rebuildMethod;
        float fixDelay = 0.025f;

        private enum RebuildMethod { ForceRebuild, MarkRebuild }

        private void OnEnable()
        {
#if UNITY_EDITOR
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            if (Application.isPlaying == false) { return; }
#endif
            if (fixWithDelay == false && fixOnEnable == true && rebuildMethod == RebuildMethod.ForceRebuild) { ForceRebuild(); }
            else if (fixWithDelay == false && fixOnEnable == true && rebuildMethod == RebuildMethod.MarkRebuild) { MarkRebuild(); }
            else if (fixWithDelay == true) { StartCoroutine(FixDelay()); }
        }


        public void FixLayout()
        {
            Debug.Log("This is being called");
            if (fixWithDelay == false && rebuildMethod == RebuildMethod.ForceRebuild) { ForceRebuild(); }
            else if (fixWithDelay == false && rebuildMethod == RebuildMethod.MarkRebuild) { MarkRebuild(); }
            else { StartCoroutine(FixDelay()); }
        }

        void ForceRebuild()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        void MarkRebuild()
        {
            LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
        }

        IEnumerator FixDelay()
        {
            yield return new WaitForSecondsRealtime(fixDelay);

            if (rebuildMethod == RebuildMethod.ForceRebuild) { ForceRebuild(); }
            else if (rebuildMethod == RebuildMethod.MarkRebuild) { MarkRebuild(); }
        }
    }
}