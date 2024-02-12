using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "UIManager", menuName = "ScriptableObject/UI/UIManager")]
    public class SO_UIManager : ScriptableObject
    {
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public AudioClip notificationSound;
    }
}