using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class SOSShipVisualSpawner : MonoBehaviour
{
    private static SOSShipVisualSpawner instance;
    public static SOSShipVisualSpawner Instance {  get { return instance; } }

    [SerializeField] private GameObject shipIconPrefab;
    [SerializeField] private GameObject SOSShipHolder;

    private Universe universe;
    public bool on;

    private void Awake()
    {

        
    }

    public void Init()
    {
        if (instance != null)
        {
            DestroyImmediate(instance);
            instance = this;
        }
        else
        {
            instance = this;
        }
    }


    // Called by Universe Instance when a chunk is rendered, and a ship is found.
    public void SpawnAShip(UniverseChunkSector sector, Universe universe)
    {
        if (!on)
        {
            return;
        }

        GameObject sosShipVisual = Instantiate(shipIconPrefab, sector.spawnPoint, Quaternion.identity, SOSShipHolder.transform);

        SOSShipVisual sosShip = sosShipVisual.GetComponent<SOSShipVisual>();
        sosShip.universe = universe;
    }
}
