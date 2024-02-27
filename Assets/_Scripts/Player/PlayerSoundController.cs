using System.Collections;
using UnityEngine;

namespace CallSOS.Player
{
    [RequireComponent (typeof(AudioSource))]
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField] internal float stepSpeed;
        [SerializeField] internal float m_WalkingInterval;
        [SerializeField][Range(0f, 1f)] internal float m_WalkingVolume;
        [SerializeField] internal float m_SprintingInterval;
        [SerializeField][Range(0f, 1f)] internal float m_SprintingVolume;
        [SerializeField] internal float m_CrouchingInterval;
        [SerializeField][Range(0f, 1f)] internal float m_CrouchingVolume;

        [SerializeField] [Range(1f, 2f)] internal float m_footstepMultiplier;
        
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private AudioClip m_BreathingLoop;           // the sound played when character touches back on ground.

        internal float m_StepCycle;
        internal float m_NextStep;

        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioSource m_FootAudioSource;
        [SerializeField] private AudioSource m_BreathingAudioSource;

        internal void Init()
        {
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
        }

        internal void ProcessStepCycle(Vector3 velocity, PlayerState playerState)
        {
            Vector2 movementVector = GameInputPlayer.Instance.GetMovementVectorNormalized();
            float stepInterval = 0;
            float stepVolume = 1;

            switch (playerState)
            {
                case PlayerState.Walking:

                    //SetTargetVolume(targetWalkingVolume);

                    stepInterval = m_WalkingInterval;
                    stepVolume = m_WalkingVolume;
                    break;

                case PlayerState.Sprinting:

                    //SetTargetVolume(targetSprintingVolume);
                    stepInterval = m_SprintingInterval;
                    stepVolume = m_SprintingVolume;
                    break;

                case PlayerState.Crouching:

                    stepInterval = m_CrouchingInterval;
                    stepVolume = m_CrouchingVolume;
                    break;
            }

            if (velocity.sqrMagnitude > 0 && (movementVector.x != 0 || movementVector.y != 0))
            {
                m_StepCycle += (velocity.magnitude + stepSpeed) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) return;

            m_NextStep = m_StepCycle + stepInterval;

            PlayFootstepAudio(stepVolume);
        }

        internal void PlayFootstepAudio(float stepVolume)
        {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_FootAudioSource.clip = m_FootstepSounds[n];
            m_FootAudioSource.PlayOneShot(m_FootAudioSource.clip, stepVolume);

            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_FootAudioSource.clip;
        }

        internal void PlayJumpSound()
        {
            m_FootAudioSource.clip = m_JumpSound;
            m_FootAudioSource.Play();
        }

        internal void PlayLandingSound()
        {
            m_FootAudioSource.clip = m_LandSound;
            m_FootAudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        //////////////////////////////////////
        ///
        ///            SPRINTING
        /// 
        //////////////////////////////////////
        
        private float targetWalkingVolume = 0f; // Adjust these values as needed
        private float targetSprintingVolume = 1.0f;
        private float volumeChangeSpeed = 0.1f; // Adjust this speed as needed

        public float currentVolume = 0.0f;
        private Coroutine volumeCoroutine;

        private void SetTargetVolume(float targetVolume)
        {
            if(volumeCoroutine != null)
                StopCoroutine(volumeCoroutine);

            volumeCoroutine = StartCoroutine(ChangeVolumeOverTime(targetVolume));

        }

        private IEnumerator ChangeVolumeOverTime(float targetVolume)
        {
            while (Mathf.Abs(currentVolume - targetVolume) > 0.01f)
            {
                currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, volumeChangeSpeed * Time.deltaTime);
                PlayBreathingAudio(currentVolume);
                yield return null;
            }
        }

        internal void PlayBreathingAudio(float volume)
        {
            if(m_BreathingAudioSource != null && m_BreathingLoop != null)
            {
                m_BreathingAudioSource.volume = volume;
                if (!m_BreathingAudioSource.isPlaying)
                {
                    m_BreathingAudioSource.clip = m_BreathingLoop;
                    m_BreathingAudioSource.loop = true;
                    m_BreathingAudioSource.Play();
                }
            }
        }
    }
}