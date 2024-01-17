using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.iOS;

public enum SpawnType
{
    Text,
    Sphere,
    Both
}

public class UniverseController : MonoBehaviour
{
    [SerializeField] private SerializedChunks serializedChunks;
    [SerializeField] private GameObject ship;
    internal Universe universe;

    public TextMeshProUGUI currentChunkText;
    public int numberOfChunksX;
    public int numberOfChunksY;

    public int numberOfSectorsXY;
    public int sectorGuiSize;
    public GameObject ShipMovement;
    public SpawnType spawnType;


    private void Start()
    {
        universe = new Universe(serializedChunks, numberOfSectorsXY, sectorGuiSize, numberOfChunksX, numberOfChunksY, spawnType);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = UtilsClass.GetMouseWorldPosition();

            Debug.Log(pos);
            universe.GetChunk(pos);
            
            //UniverseChunkSector sector = universe.GetChunkSector(pos);

            //universe.AddShip(pos);
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
