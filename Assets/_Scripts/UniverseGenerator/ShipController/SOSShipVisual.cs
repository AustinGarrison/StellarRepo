using HeathenEngineering;
using UnityEngine;

public class SOSShipVisual : MonoBehaviour
{
    internal Universe universe;

    public Sprite[] sosShipSprites = null;
    public Vector3 worldPosition = Vector3.zero;
    public int globalSectorPosX;
    public int globalSectorPosY;
    public int localSectorPosX;
    public int localSectorPosY;
    public int ChunkPosX;
    public int ChunkPosY;

    public Rigidbody2D rigidBody;
    public SpriteRenderer spriteRenderer;
    public PlayerShipNavigator playerShipNavigator;

    public Color startColor = Color.white;
    public Color endColor = Color.clear;
    public float fadeDuration = 2f;
    private float elapsedTime = 0f;
    private bool inPlayerProximity;

    private void Start()
    {
        spriteRenderer.sprite = sosShipSprites[0];
        playerShipNavigator = PlayerShipNavigator.Instance;
        spriteRenderer.color = endColor;
        elapsedTime = fadeDuration;
    }
    
    private void Update()
    {
        GetChunkSector();

        if (!inPlayerProximity)
            DisappearTimer();
    }

    private void FixedUpdate()
    {
        UpdatePositions();
    }

    private void DisappearTimer()
    {
        if(elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
        }
    }

    private void UpdatePositions()
    {
        // Apply force based on the combined movement directions
        float adjustedThrustSpeed = PlayerShipNavigator.Instance.thrustSpeed / transform.localScale.x;
        rigidBody.AddForce(-PlayerShipNavigator.Instance.movementDirection * adjustedThrustSpeed);
    }

    private void GetChunkSector()
    {
        UniverseChunk chunk = universe.GetChunk(transform.position);

        if (chunk == null)
        {
            Destroy(this.gameObject);
            return;
        }

        //UniverseChunkSector sector = universe.GetChunkSector(transform.position);

        //localSectorPosX = sector.sectorPosition.x;
        //localSectorPosY = sector.sectorPosition.y;
        //ChunkPosX = chunk.localPositionX;
        //ChunkPosY = chunk.localPositionY;
    }

    public void SetColor(Color color)
    {
        elapsedTime = 0f;
        spriteRenderer.color = color;
    }

    public void ShowSelectedSprite()
    {
        SetColor(Color.white);
        spriteRenderer.sprite = sosShipSprites[1];
        inPlayerProximity = true;
    }

    public void ShowUnselectedSprite()
    {
        spriteRenderer.sprite = sosShipSprites[0];
        inPlayerProximity = false;
    }
}
