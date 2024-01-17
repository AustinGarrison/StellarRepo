using FishNet.Demo.AdditiveScenes;
using FishNet.Example.Authenticating;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShipNavController : MonoBehaviour
{
    public static PlayerShipNavController Instance { get { return instance; } }
    private static PlayerShipNavController instance;
    public UniverseController universeController;

    private Rigidbody2D shipRotationRB;
    private Rigidbody2D shipMovementRB;
    public GameObject shipMovementGameObject;

    private float turnDirection;

    public float rotationSpeed = 0.1f;

    public float thrustSpeed = 1f;

    public Vector2 movementDirection = Vector2.zero;

    [SerializeField] private GameObject shipVisual;
    [SerializeField] private PlayerMovementTracker shipMovement;
    [SerializeField] private SOSShipVisualSpawner iconSpawner;
    [SerializeField] private Transform spawnPoint;
    private bool movingUp;
    public bool on;

    public delegate void SpacebarPressedDelegate(Vector2 MovementDirection, float thrustSpeed);
    public static event SpacebarPressedDelegate OnSpacebarPressed;
    private Universe universe;

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(instance);
        }
        else
        {
            instance = this;
            iconSpawner.Init();
            shipRotationRB = shipVisual.GetComponent<Rigidbody2D>();
            shipMovementRB = shipMovement.GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        this.universe = universeController.universe;
        InitShipVisuals();
    }

    private void Update()
    {
        if (!on) return;

        GetKeyBinds();


        if (OnSpacebarPressed != null)
        {
            OnSpacebarPressed(this.movementDirection, this.thrustSpeed);
        }
    }

    private void FixedUpdate()
    {
        UpdateRotaions();
        UpdatePositions();
    }

    private void GetKeyBinds()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turnDirection = -1f;
        }
        else
        {
            turnDirection = 0f;
        }

        movingUp = Input.GetKey(KeyCode.W);
    }

    private void UpdateRotaions()
    {
        if (turnDirection != 0f)
        {
            shipMovementRB.AddTorque(rotationSpeed * turnDirection);
            shipRotationRB.AddTorque(rotationSpeed * turnDirection);
        }
    }


    private void UpdatePositions()
    {
        Vector2 localUp = shipMovement.transform.up;

        movementDirection = Vector2.zero;

        if (movingUp)
        {
            movementDirection += localUp;
        }

        // Normalize the direction to ensure consistent speed in all directions
        movementDirection.Normalize();

        // Apply force based on the combined movement directions
        shipMovementRB.AddForce(movementDirection * thrustSpeed);
    }

    private void InitShipVisuals()
    {
        shipMovement.Init(spawnPoint, universe);

        shipMovement.transform.position = spawnPoint.position;
        shipMovement.gameObject.SetActive(true);
    }
}
