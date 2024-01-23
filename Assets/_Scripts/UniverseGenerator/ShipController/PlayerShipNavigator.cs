using System.Collections.Generic;
using UnityEngine;

public class PlayerShipNavigator : MonoBehaviour
{
    public static PlayerShipNavigator Instance { get { return instance; } }
    private static PlayerShipNavigator instance;

    public List<SOSShipVisual> SOSShipList;

    // Applied to Player and SOSShips for RigidBody Movement
    [SerializeField] private float ThrustSpeed = 1f;
    public float thrustSpeed { get { return ThrustSpeed; } }

    [SerializeField] private float RotationSpeed = 0.1f;
    public float rotationSpeed { get { return RotationSpeed; } }

    private Vector2 MovementDirection = Vector2.zero;
    public Vector2 movementDirection { get { return MovementDirection; } }


    [SerializeField] private Transform playerSpawnPoint;

    [SerializeField] private GameObject playerShipRotateVisual;
    [SerializeField] private GameObject shipIconPrefab;
    [SerializeField] private GameObject SOSShipHolder;

    [SerializeField] private PlayerMovementTracker playerShipMovement;
    [SerializeField] private UniverseManager universeManager;

    private bool acceleration;
    private float turnDirection;
    private Rigidbody2D shipRotationRB;
    private Rigidbody2D shipMovementRB;
    private Universe universe;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(instance);
        }
        else
        {
            instance = this;
            shipRotationRB = playerShipRotateVisual.GetComponent<Rigidbody2D>();
            shipMovementRB = playerShipMovement.GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        this.universe = universeManager.universe;
        InitShipVisuals();
    }

    private void Update()
    {
        GetPlayerMovement();
    }

    private void FixedUpdate()
    {
        GetMovementDirection();
        UpdateRotations();
        UpdatePositions();
    }

    private void GetPlayerMovement()
    {
        if (GameInputStarMap.Instance.GetRotateRight() > 0)
        {
            turnDirection = -1f;
        }
        else if (GameInputStarMap.Instance.GetRotateLeft() > 0)
        {
            turnDirection = 1f;
        }
        else
        {
            turnDirection = 0f;
        }

        acceleration = GameInputStarMap.Instance.GetAccelerating() > 0;
    }

    private void UpdateRotations()
    {
        if (turnDirection != 0f)
        {
            // Calculate adjusted rotation speed based on the scale of the object
            float adjustedRotationSpeed = RotationSpeed / transform.localScale.x;

            shipMovementRB.AddTorque(adjustedRotationSpeed * turnDirection);
            shipRotationRB.AddTorque(adjustedRotationSpeed * turnDirection);

            //shipMovementRB.AddTorque(rotationSpeed * turnDirection);
            //shipRotationRB.AddTorque(rotationSpeed * turnDirection);
        }
    }

    private void GetMovementDirection()
    {
        Vector2 localUp = playerShipMovement.transform.up;

        MovementDirection = Vector2.zero;

        if (acceleration)
            MovementDirection += localUp;

        // Normalize the direction to ensure consistent speed in all directions
        MovementDirection.Normalize();
    }

    private void UpdatePositions()
    {
        // Apply force based on the combined movement directions
        float adjustedThrustSpeed = (Instance.thrustSpeed / shipMovementRB.transform.localScale.x);
        //shipMovementRB.AddForce(movementDirection * thrustSpeed);
        shipMovementRB.AddForce(movementDirection * adjustedThrustSpeed);
    }

    private void InitShipVisuals()
    {
        playerShipMovement.Init(playerSpawnPoint, universe);

        playerShipMovement.transform.position = playerSpawnPoint.position;
        playerShipMovement.gameObject.SetActive(true);
    }

    // Called by Universe Instance when a chunk is rendered, and a ship is found.
    public void SpawnSOSShip(UniverseChunkSector sector, Universe universe)
    {
        //GameObject sosShipVisual = Instantiate(shipIconPrefab, Vector3.zero, Quaternion.identity, SOSShipHolder.transform);
        GameObject sosShipVisual = Instantiate(shipIconPrefab, sector.spawnPoint, Quaternion.identity, SOSShipHolder.transform);

        SOSShipVisual sosShip = sosShipVisual.GetComponent<SOSShipVisual>();
        sosShip.universe = universe;
    }
}
