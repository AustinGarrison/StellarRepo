using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class PanelButton : MonoBehaviour
    {
        // Events
        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onHover = new UnityEvent();
        public UnityEvent onLeave = new UnityEvent();
        public UnityEvent onSelect = new UnityEvent();

        public void SetSelected(bool value)
        {

        }
    }
}