using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CallSOS.Utilities
{
    public class KeyPressEvent : MonoBehaviour
    {
        public InputAction keyCode;
        public UnityEvent eventTest;

        void Update()
        {
            if (keyCode.triggered)
            {
                eventTest.Invoke();
            }    
        }
    }
}