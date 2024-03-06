using CallSOS.Player.Interaction;
using CallSOS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestInteract : OperationAction
{
    public UnityEvent testInteractAction = new UnityEvent();

    [SerializeField] private AudioClip[] m_Sounds;
    [SerializeField] private AudioSource m_AudioSource;

    public override void InteractWithOperation()
    {
        testInteractAction?.Invoke();
    }

    public void PlayAudio(float stepVolume)
    {
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_Sounds.Length);
        m_AudioSource.clip = m_Sounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip, stepVolume);

        // move picked sound to index 0 so it's not picked next time
        m_Sounds[n] = m_Sounds[0];
        m_Sounds[0] = m_AudioSource.clip;
    }

}
