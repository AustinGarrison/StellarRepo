using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class NavigationBar : MonoBehaviour
    {


        List<PanelButton> buttons = new List<PanelButton>();

        public void LitButtons(PanelButton source = null)
        {
            foreach (PanelButton btn in buttons)
            {
                if (btn.isSelected || (source != null && btn == source))
                    continue;

                btn.IsInteractable(true);
            }
        }

        public void DimButtons(PanelButton source)
        {
            foreach (PanelButton btn in buttons)
            {
                if (btn.isSelected || btn == source)
                    continue;

                btn.IsInteractable(false);
            }
        }
    }
}