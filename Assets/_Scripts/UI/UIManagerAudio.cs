using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace UI.Audio
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class UIManagerAudio : MonoBehaviour
    {
        // Static Instance
        public static UIManagerAudio instance;

        public SO_UIManager UIManagerAsset;
        [SerializeField] private AudioMixer audioMixer;
        public AudioSource audioSource;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if(audioSource == null)
            {
                gameObject.GetComponent<AudioSource>();
            }
        }
    }

}