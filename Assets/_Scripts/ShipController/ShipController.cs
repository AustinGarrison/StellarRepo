using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float thrustSpeed = 1f;
    private bool thrusting;

    public float rotationSpeed = 0.1f;
    private float turnDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        Debug.Log(thrusting);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            turnDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            turnDirection = -1f;
        }
        else
        {
            turnDirection = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (thrusting)
        {
            rb.AddForce(transform.up * thrustSpeed);
        }

        if (turnDirection != 0f)
        {
            rb.AddTorque(rotationSpeed * turnDirection);
        }
    }
}
