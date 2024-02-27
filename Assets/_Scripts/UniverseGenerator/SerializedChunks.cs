using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializedChunks : MonoBehaviour, ISerializationCallbackReceiver
{
    private static SerializedChunks instance;
    public static SerializedChunks Instance { get { return instance; } }

    public UniverseChunk[,] _activeChunks;

    public int chunksLengthX, chunksLengthY;
    [SerializeField] private List<ChunkStruct<UniverseChunk>> ActiveChunks;


    [System.Serializable]
    struct ChunkStruct<TElement>
    {
        public int localPosX;
        public int localPosY;
        public TElement Chunk;
        public ChunkStruct(int posX, int posY, TElement element)
        {
            localPosX = posX;
            localPosY = posY;
            Chunk = element;
        }
    }

    public void SetSectorLengths(int numOfSectors)
    {

        for (int i = 0; i < _activeChunks.GetLength(0); i++)
        {
            for (int j = 0; j < _activeChunks.GetLength(1); j++)
            {
                _activeChunks[i, j].numberOfSectorsXY = numOfSectors;
            }
        }
        
    }

    public void OnBeforeSerialize()
    {
        ActiveChunks = new List<ChunkStruct<UniverseChunk>>();
        for (int i = 0; i < _activeChunks.GetLength(0); i++)
        {
            for (int j = 0; j < _activeChunks.GetLength(1); j++)
            {
                ActiveChunks.Add(new ChunkStruct<UniverseChunk>(i, j, _activeChunks[i, j]));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        _activeChunks = new UniverseChunk[chunksLengthX, chunksLengthY];
        foreach (var chunk in ActiveChunks)
        {
            _activeChunks[chunk.localPosX, chunk.localPosY] = chunk.Chunk;
        }
    }

}

[System.Serializable]
public class UniverseChunk
{
    public event EventHandler<OnSectorValueChangedEventArgs> OnSectorValueChanged;
    public class OnSectorValueChangedEventArgs : EventArgs
    {
        public int posInSectorX;
        public int posInSectorY;
        public UniverseChunkSector sector;
    }

    public int localPositionX;
    public int localPositionY;
    
    public int globalPositionX;
    public int globalPositionY;

    public int numberOfSectorsXY;
    public int sectorGUISize;

    private UniverseChunkSector chunkSector;
    public UniverseChunkSector[,] sectorsArray;

    Vector3 originOffset = Vector3.zero;

    private Vector2Int sector;
    private bool hasShip = false;

    public void SetupChunk(int globalX, int globalY, int localX, int localY, int numberOfSectorsXY, int sectorGUISize, out bool hasShip)
    {
        this.hasShip = false;

        globalPositionX = globalX;
        globalPositionY = globalY;

        localPositionX = localX;
        localPositionY = localY;

        this.numberOfSectorsXY = numberOfSectorsXY;
        this.sectorGUISize = sectorGUISize;

        GenerateSectors();
        hasShip = this.hasShip;
    }


    public void GenerateSectors()
    {
        originOffset = new Vector3(globalPositionX * numberOfSectorsXY * sectorGUISize, globalPositionY * numberOfSectorsXY * sectorGUISize, 0f);

        sectorsArray = new UniverseChunkSector[numberOfSectorsXY, numberOfSectorsXY];

        for (sector.x = 0; sector.x < numberOfSectorsXY; sector.x++)
        {
            for (sector.y = 0; sector.y < numberOfSectorsXY; sector.y++)
            {
                chunkSector = new UniverseChunkSector(globalPositionX, globalPositionY, sector, numberOfSectorsXY);

                sectorsArray[sector.x, sector.y] = chunkSector;

                if (chunkSector.shipExists && hasShip == false)
                {
                    hasShip = true;
                }
            }
        }
    }

    public void InitSectorChangedEvent()
    {
        OnSectorValueChanged += (object sender, OnSectorValueChangedEventArgs eventArgs) =>
        {
            //debugTextArray[
            //    eventArgs.sector.sectorPosition.x, eventArgs.sector.sectorPosition.y] =
            //    sectorsArray[eventArgs.sector.sectorPosition.x, eventArgs.sector.sectorPosition.y]?.GetIsShip();
        };
    }

    public void TriggerSectorChanged(UniverseChunkSector sector)
    {
        if(OnSectorValueChanged != null)
        {
            OnSectorValueChanged(this, new OnSectorValueChangedEventArgs { sector = sector });
        }
    }

    public Vector3 GetSectorPosition(int x, int y)
    {
        return new Vector3(x, y) * sectorGUISize + originOffset;
    }
}
