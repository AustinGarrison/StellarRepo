using UnityEngine;

namespace CallSOS.Player
{
    [RequireComponent (typeof(AudioSource))]
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField] internal float m_StepInterval;
        
        [SerializeField][Range(0f, 1f)] internal float m_RunstepLenghten;
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.


        internal float m_StepCycle;
        internal float m_NextStep;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        private AudioSource m_AudioSource;

        internal void Init()
        {
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_AudioSource = GetComponent<AudioSource>();
        }

        internal void ProcessStepCycle(Vector3 velocity, float speed)
        {
            Vector2 movementVector = GameInputPlayer.Instance.GetMovementVectorNormalized();

            if (velocity.sqrMagnitude > 0 && (movementVector.x != 0 || movementVector.y != 0))
            {
                m_StepCycle += (velocity.magnitude + (speed *  1f )) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) return;

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootstepAudio();
        }

        internal void PlayFootstepAudio()
        {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        internal void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        internal void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

    }
}