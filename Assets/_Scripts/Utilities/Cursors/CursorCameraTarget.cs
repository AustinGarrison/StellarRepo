using FishNet.Object;
using UnityEngine;

namespace CallSOS.Utilities
{
    public class CursorCameraTarget : NetworkBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] Transform player;
        [SerializeField] float threshold;
        [SerializeField] float minSpeed;
        [SerializeField] float maxSpeed;

        [SerializeField] float speedByDistance = 0f;

        void Update()
        {
            if (!base.IsOwner)
            {
                return;
            }

            // Get the mouse position in world space
            Vector3 mousePosition = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerCamera.transform.position.y - player.position.y));

            // Calculate the target position for the camera
            Vector3 direction = mousePosition - player.position;

            // Increase camera speed based on distance mouse is from player
            speedByDistance = Mathf.Clamp(Vector3.Distance(mousePosition, player.position) / 4f, minSpeed, maxSpeed);

            // Clamp the target position within the threshold
            direction = Vector3.ClampMagnitude(direction, threshold);

            // Set the target position to the target position
            Vector3 targetPosition = player.position + direction;

            // Interpolate towards the target 
            Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, player.position.y, targetPosition.z), Time.deltaTime * speedByDistance);

            transform.position = newPosition;
        }
    }
}