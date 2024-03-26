using FishNet.Demo.AdditiveScenes;
using FishNet.Example.ColliderRollbacks;
using System;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public float moveSpeed = 3f;

    [Header("Cam Distance Clamps")]
    public float minMovePercent = 0.125f;
    public float maxMovePercent = 0.13f;
    public float minDistance = 0f;
    public float maxDistance = 3.5f;


    private float movementPercentage = 0.3f;

    internal Transform player;
    private Camera playerCamera;
    private float cameraHeight;

    private Vector3 cameraFollowPosition;
   

    private void Start()
    {
        playerCamera = Camera.main;

        cameraHeight = playerCamera.transform.position.y - player.position.y;

        //Teleport to player
        cameraFollowPosition = GetMousePosition();
        cameraFollowPosition.y = transform.position.y;
        transform.position = cameraFollowPosition;
    }

    private void Update()
    {
        HandleMovement();
        SetMovePercentMultiplier();
    }

    private void HandleMovement()
    {
        cameraFollowPosition = GetMousePosition();
        cameraFollowPosition.y = transform.position.y;

        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;

        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        if (distance > 0)
        {

            Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * moveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

            if (distanceAfterMoving > distance)
            {
                // Overshot the target
                newCameraPosition = cameraFollowPosition;
            }

            transform.position = newCameraPosition;
        }
    }


    private void SetMovePercentMultiplier()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log(distanceFromPlayer);

        // Interpolate MovePercent between minMovePercent and maxMovePercent based on the distance
        float normalizedDistance = Mathf.Clamp01((distanceFromPlayer - minDistance) / (maxDistance - minDistance));
        movementPercentage = Mathf.Lerp(minMovePercent, maxMovePercent, normalizedDistance);
    }

    private Vector3 GetMousePosition()
    {
        // Check if the cursor is within the game screen bounds
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            // Cursor is outside the screen bounds, return a default value or handle it accordingly
            return player.transform.position; // Return zero vector as a default value
        }

        // Calulate the position of the mouse in world space
        Vector3 mousePosition = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraHeight));

        // Calculate the direction vector from the player to the mouse position
        Vector3 playerToMouseDirection = mousePosition - player.position;

        // Calculate the final position by interpolating between player position and mouse position
        return player.position + playerToMouseDirection * movementPercentage;
    }
}
