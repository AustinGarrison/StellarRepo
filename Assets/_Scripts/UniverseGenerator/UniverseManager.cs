using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Determine if SOSShips spawn as text or sprite
public enum SpawnType
{
    Text,
    Sphere,
    Both
}

public class UniverseManager : MonoBehaviour
{
    [SerializeField] private SerializedChunks serializedChunks;
    [SerializeField] private GameObject ship;
    internal Universe universe;

    public TextMeshProUGUI currentChunkText;

    [Header("Settings")]
    [ReadOnlyRunTime] public int numberOfChunksXY;
    [ReadOnlyRunTime] public int numberOfChunksY;
    [ReadOnlyRunTime] public int numberOfSectorsXY;
    [ReadOnlyRunTime] public int sectorGuiSize;
    [ReadOnlyRunTime] public SpawnType spawnType;


    private void Start()
    {
        universe = new Universe(serializedChunks, numberOfChunksXY, numberOfSectorsXY, sectorGuiSize, spawnType);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector3 pos = UtilsClass.GetMouseWorldPosition();
            
            //universe.GetChunk(pos);
            
            UniverseChunkSector sector = universe.GetChunkSector(pos);
            //Debug.Log(sector.universeSectorX + " " + sector.universeSectorY);
            //universe.AddShip(sector);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 pos = UtilsClass.GetMouseWorldPosition();            
            Debug.Log(universe.GetIsShip(pos));
        }

        string currentChunk =
            "Current Chunk: (" + (universe.currentCenterChunkX + 2) + ", " + (universe.currentCenterChunkY + 2) + ")";

        currentChunkText.text = currentChunk;
    }

    // Called from Canvas Buttons
    public void MoveMap(int dir)
    {
        Destroy(universe.visibleShipsTextParent);

        switch (dir)
        {
            case 1:
                universe.MoveChunks(Universe.MoveChunkDir.North);
                break;
            case -1:
                universe.MoveChunks(Universe.MoveChunkDir.South);
                break;
            case 2:
                universe.MoveChunks(Universe.MoveChunkDir.East);
                break;
            case -2:
                universe.MoveChunks(Universe.MoveChunkDir.West);
                break;
        }
    }

}
