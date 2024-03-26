using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeMonkeyPlayerMove : MonoBehaviour
{
    private const float SPEED = 50f;

    private Vector3 lookAtPosition;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
        }

        Vector3 moveDir = new Vector3(moveX, moveY).normalized;

        bool isIdle = moveX == 0 && moveY == 0;
        if (!isIdle)
        {
            transform.position += moveDir * SPEED * Time.deltaTime;
        }
    }
}
