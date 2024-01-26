using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        universe = new Universe(serializedChunks, numberOfChunksXY, numberOfSectorsXY, sectorGuiSize);
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
}
