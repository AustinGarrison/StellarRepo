using UnityEngine;

namespace CallSOS.Player.UI
{
    public class PlayerHelmetMover : MonoBehaviour
    {
        // Helmet Movement
        public float moveSpeed;
        public float multiplier;

        private Vector3 startPosition = Vector3.zero;
        private Vector3 targetPosition;
        private Vector3 moveDirection;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            MoveHelmetHudImage();
        }

        private void MoveHelmetHudImage()
        {
            Vector2 look = GameInputPlayer.Instance.GetLookVector();

            look.x = Mathf.Clamp(look.x, -1f, 1f);
            look.y = Mathf.Clamp(look.y, -1f, 1f);

            moveDirection.z = look.x;
            moveDirection.y = look.y;

            moveDirection /= multiplier;

            if (moveDirection == Vector3.zero)
            {
                targetPosition = startPosition;
            }

            moveDirection.y *= -1;

            targetPosition = startPosition + moveDirection;

            // Use Vector3.Lerp to smoothly interpolate between current position and target position.
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}