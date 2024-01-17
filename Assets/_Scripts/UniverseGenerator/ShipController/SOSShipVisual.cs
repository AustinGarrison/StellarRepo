using UnityEngine;

public class SOSShipVisual : MonoBehaviour
{
    internal Universe universe;
    private PlayerShipNavController controller;
    public Vector3 worldPosition = Vector3.zero;
    public int SectorPosX;
    public int SectorPosY;
    public int ChunkPosX;
    public int ChunkPosY;

    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = -PlayerShipNavController.Instance.movementDirection * PlayerShipNavController.Instance.thrustSpeed;
        // PlayerShipNavController.OnSpacebarPressed += OnSpacebarPressed;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        // PlayerShipNavController.OnSpacebarPressed -= OnSpacebarPressed;
    }

    private void Update()
    {

        UpdatePositions();

        UniverseChunk chunk = universe.GetChunk(transform.position);

        if (chunk == null)
        {
            Destroy(this.gameObject);
            return;
        }
            
        UniverseChunkSector sector = universe.GetChunkSector(transform.position);

        SectorPosX = sector.sectorPosition.x;
        SectorPosY = sector.sectorPosition.y;
        ChunkPosX = chunk.localPositionX;
        ChunkPosY = chunk.localPositionY;
    }

    private void UpdatePositions()
    {
        // Apply force based on the combined movement directions
        rb.AddForce(-PlayerShipNavController.Instance.movementDirection * PlayerShipNavController.Instance.thrustSpeed);
    }

    private void OnSpacebarPressed(Vector2 movementDirection, float thrustSpeed)
    {


    }
}
