
using Newtonsoft.Json.Bson;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Universe
{
    public enum MoveChunkDir
    {
        None,
        North,
        South,
        East,
        West
    }

    SerializedChunks activeChunksList;
    Vector2Int centerChunkOffset = new Vector2Int(0, 0);

    internal int currentCenterChunkX;
    internal int currentCenterChunkY;

    int sectorGUISize;
    int numberOfSectorsXY;
    int numChunksX, numChunksY;


    public Universe(SerializedChunks activeChunksList, int numChunkXY, int numberOfSectorsXY, int sectorGUISize)
    {
        this.activeChunksList = activeChunksList;
        this.numberOfSectorsXY = numberOfSectorsXY;
        this.sectorGUISize = sectorGUISize;
        this.numChunksX = numChunkXY;
        this.numChunksY = numChunkXY;

        currentCenterChunkX = 0;
        currentCenterChunkY = 0;

        StartingChunks();

        currentCenterChunkX = 0;
        currentCenterChunkY = 0;
    }

    private void StartingChunks()
    {
        if (activeChunksList != null && activeChunksList._activeChunks != null)
        {
            for (int x = 0; x < activeChunksList._activeChunks.GetLength(0); x++)
            {
                for (int y = 0; y < activeChunksList._activeChunks.GetLength(1); y++)
                {
                    SetupChunk(x, y);
                }
            }
        }
    }

    public void MoveChunks(MoveChunkDir newDirection)
    {
        switch (newDirection)
        {
            case MoveChunkDir.North:
                currentCenterChunkY += 1;
                break;
            case MoveChunkDir.South:
                currentCenterChunkY -= 1;
                break; 
            case MoveChunkDir.East:
                currentCenterChunkX += 1;
                break;
            case MoveChunkDir.West:
                currentCenterChunkX -= 1;
                break;
        }

        GenerateNewChunks(newDirection);
    }

    private void GenerateNewChunks(MoveChunkDir direction)
    {
        UniverseChunk[,] oldChunks = activeChunksList._activeChunks;

        int arrayLength = -1;

        // Get length or height
        if (direction == MoveChunkDir.North || direction == MoveChunkDir.South)
        {
            arrayLength = activeChunksList._activeChunks.GetLength(0);
        }
        else if (direction == MoveChunkDir.East || direction == MoveChunkDir.West)
        {
            arrayLength = activeChunksList._activeChunks.GetLength(1);
        }

        // Shift all rows or columns
        oldChunks = ShiftKeptChunks(direction, arrayLength, oldChunks);

        // Remove remove row or column that is going to be regenerated
        // Sould this even matter? They are being overwritten anyway?
        //oldChunks = RemoveOverwriteChunks(direction, arrayLength, oldChunks);

        // Set universe array to new array
        activeChunksList._activeChunks = oldChunks;

        // Generate the new chunks
        if (direction == MoveChunkDir.North || direction == MoveChunkDir.South)
        {
            NewChunksVertical(direction, arrayLength);
        }
        else if (direction == MoveChunkDir.East || direction == MoveChunkDir.West)
        {
            NewChunksHorizontal(direction, arrayLength);
        }
    }

    private UniverseChunk[,] RemoveOverwriteChunks(MoveChunkDir direction, int arrayLength, UniverseChunk[,] chunksArray)
    {
        switch (direction)
        {
            case MoveChunkDir.North:

                for (int x = 0; x < arrayLength; x++)
                {
                    chunksArray[x, 0] = null;
                }
                break;

            case MoveChunkDir.South:

                for (int x = 0; x < arrayLength; x++)
                {
                    chunksArray[x, arrayLength - 1] = null;
                }
                break;

            case MoveChunkDir.East:
                for (int y = 0; y < arrayLength; y++)
                {
                    chunksArray[arrayLength - 1, y] = null;
                }
                break;

            case MoveChunkDir.West:

                for (int y = 0; y < arrayLength; y++)
                {
                    chunksArray[0, y] = null;
                }
                break;

            default:
                break;
        }

        return chunksArray;
    }

    private UniverseChunk[,] ShiftKeptChunks(MoveChunkDir direction, int arrayLength, UniverseChunk[,] chunksArray)
    {
        switch (direction)
        {
            case MoveChunkDir.North:

                for (int y = 0; y < arrayLength - 1; y++)
                {
                    for (int x = 0; x < arrayLength; x++)
                    {
                        chunksArray[x, y] = chunksArray[x, y + 1];
                        chunksArray[x, y].globalPositionY += 1;
                    }
                }
                break;

            case MoveChunkDir.South:

                for (int y = arrayLength - 1; y > 0; y--)
                {
                    for (int x = 0; x < arrayLength; x++)
                    {
                        chunksArray[x, y] = chunksArray[x, y - 1];
                        chunksArray[x, y].globalPositionY -= 1;
                    }
                }
                break;

            case MoveChunkDir.East:

                for (int x = 0; x < arrayLength - 1; x++)
                {
                    for (int y = 0; y < arrayLength; y++)
                    {
                        chunksArray[x, y].sectorsArray = chunksArray[x + 1, y].sectorsArray;
                        chunksArray[x, y].globalPositionX += 1;
                    }
                }
                break;

            case MoveChunkDir.West:

                for (int x = arrayLength - 1; x > 0; x--)
                {
                    for (int y = 0; y < arrayLength; y++)
                    {
                        chunksArray[x, y].sectorsArray = chunksArray[x - 1, y].sectorsArray;
                        chunksArray[x, y].globalPositionX -= 1;
                    }
                }
                break;

            default:
                break;
        }

        return chunksArray;
    }

    private void NewChunksHorizontal(MoveChunkDir direction, int arrayLength )
    {        
        // Will either be 0 or 4
        int x = NewChunkSectionValue(direction, arrayLength);

        // We moved right or left, so create new chunks on the Y axis
        for (int y = 0; y < arrayLength; y++)
            SetupChunk(x, y);
        
    }

    private void NewChunksVertical(MoveChunkDir direction, int arrayLength)
    {
        // Will either be 0 or 4
        int y = NewChunkSectionValue(direction, arrayLength);

        // We moved up or down, so create new chunks on the X axis
        for (int x = 0; x < arrayLength; x++)
            SetupChunk(x, y);
    }

    private int NewChunkSectionValue(MoveChunkDir direction, int arrayLength)
    {
        int sectionValue = -1;

        if (direction == MoveChunkDir.West || direction == MoveChunkDir.South)
            sectionValue = 0;
        else if (direction == MoveChunkDir.East || direction == MoveChunkDir.North)
            sectionValue = arrayLength - 1;

        return sectionValue;
    }

    private void SetupChunk(int x, int y)
    {
        activeChunksList._activeChunks[x, y].SetupChunk(

            // Global Positions
            x + currentCenterChunkX,
            y + currentCenterChunkY,

            // Local Positions, 
            x,
            y,
            numberOfSectorsXY,

            // Settings
            sectorGUISize,
            out bool chunkHasShip);

        if (chunkHasShip)
        {
            // Render the chunks to the screen, Only generates one row/column 
            ChunkVisuals(activeChunksList._activeChunks[x, y]);
        }
    }

    #region GetChunkInfo

    public UniverseChunkSector GetChunkSector(Vector3 worldPos, out UniverseChunk chunk)
    {
        int chunkX, chunkY;
        int sectorX, sectorY;

        GetChunkXY(worldPos, out chunkX, out chunkY);

        chunk = GetChunk(chunkX, chunkY);

        if (chunk == null)
            return null;

        GetSectorXY(worldPos, out sectorX, out sectorY);
        UniverseChunkSector chunkSector = GetChunkSector(chunk, sectorX, sectorY);

        if (chunkSector == null)
            return null;

        return chunkSector;
    }

    private void GetChunkXY(Vector3 worldPos, out int chunkX, out int chunkY)
    {
        chunkX = Mathf.FloorToInt(worldPos.x / (sectorGUISize * numberOfSectorsXY));
        chunkY = Mathf.FloorToInt(worldPos.y / (sectorGUISize * numberOfSectorsXY));
    }

    public UniverseChunk GetChunk(Vector3 worldPosition)
    {
        int x, y;
        GetChunkXY(worldPosition, out x, out y);
        //Debug.Log("ChunkX: " + x + " ChunkY: " + y);
        return GetChunk(x, y);
    }

    public UniverseChunk GetChunk(int x, int y)
    {
        // x = 0 and y = 2
        if (x < 0 || y < 0 || x > numChunksX - 1 || y > numChunksY - 1)
        {
            return null;
        }
        else
        {
            return activeChunksList._activeChunks[x, y];
        }


        if (x >= 0 && y >= 0 && x < numChunksX && y < numChunksY)
        {


        }
        else
        {
            return default(UniverseChunk);
        }
    }

    public UniverseChunkSector GetChunkSector(Vector3 worldPos)
    {
        int chunkX, chunkY;
        int sectorX, sectorY;

        GetChunkXY(worldPos, out chunkX, out chunkY);
        UniverseChunk chunk = GetChunk(chunkX, chunkY);
        Debug.Log("GlobalX: " + chunk.globalPositionX + " GlobalY: " + chunk.globalPositionY);
        Debug.Log("LocalX: " + chunk.localPositionX + " LocalY: " + chunk.localPositionY);

        //GetSectorXY(worldPos, out sectorX, out sectorY);
        //UniverseChunkSector chunkSector = GetChunkSector(chunk, sectorX, sectorY);

        //return chunkSector;
        return null;
    }


    public UniverseChunkSector GetChunkSector(UniverseChunk chunk, int x, int y)
    {
        //Not Done
        if (true)
        {
            x = (x - (chunk.localPositionX * numberOfSectorsXY));
            y = (y - (chunk.localPositionY * numberOfSectorsXY));
            return chunk.sectorsArray[x, y];
        }
        else
        {
            //return default(UniverseChunkSector);
        }
    }

    private void GetSectorXY(Vector3 worldPos, out int sectorX, out int sectorY)
    {
        sectorX = Mathf.FloorToInt(worldPos.x / sectorGUISize);
        sectorY = Mathf.FloorToInt(worldPos.y / sectorGUISize);
    }

    #endregion

    #region GUIVisualization

    private void ChunkVisuals(UniverseChunk chunkToRender)
    {
        for (int sectorX = 0; sectorX < chunkToRender.sectorsArray.GetLength(0); sectorX++)
        {
            for (int sectorY = 0; sectorY < chunkToRender.sectorsArray.GetLength(1); sectorY++)
            {
                VisualizeSectorShips(chunkToRender, sectorX, sectorY);
                //VisualizeSectorRandomInt(chunkToRender, sectorX, sectorY);
            }
        }

        // Draw Lines surrounding the chunk
        // Currently Grid is displayed as a sprite
        //DrawChunkBorders(chunkToRender);
    }

    private void VisualizeSectorShips(UniverseChunk chunk, int x, int y)
    {
        if (chunk.sectorsArray[x, y]?.GetIsShip() == "True")
        {
            Vector3 textPosition = chunk.GetSectorPosition(x, y);
            
            textPosition.x -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkX;
            textPosition.y -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkY;//* centerChunkOffset.y;

            chunk.sectorsArray[x, y].spawnPoint = textPosition + new Vector3(sectorGUISize, sectorGUISize, 0) / 2;

            PlayerShipNavigator.Instance.SpawnSOSShip(chunk.sectorsArray[x, y], this);
            
        }

        chunk.InitSectorChangedEvent();
    }

    private void VisualizeSectorRandomInt(UniverseChunk chunk, int x, int y)
    {
        Vector3 textPosition = chunk.GetSectorPosition(x, y); 
        
        textPosition.x -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkX;
        textPosition.y -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkY;//* centerChunkOffset.y;

        chunk.InitSectorChangedEvent();
    }

    #endregion

    #region Debugging
    public UniverseChunk AddShip(Vector3 worldPos)
    {
        UniverseChunk chunk;
        UniverseChunkSector chunkSector = GetChunkSector(worldPos, out chunk);

        if(chunk == null)
            return null;

        Debug.Log("Added a ship to Chunk: X-" + chunk.localPositionX + " Y-" + chunk.localPositionY + "\n" +
            " At Sector: X-" + chunkSector.sectorPosition.x + " Y-" + chunkSector.sectorPosition.y +
            " SectorGlobals: X-" + chunkSector.universeSectorX+ " Y-" + chunkSector.universeSectorY);

        chunkSector.shipExists = true;
        chunk.TriggerSectorChanged(chunkSector);
        ChunkVisuals(chunk);
        return chunk;
    }

    public bool GetIsShip(Vector3 worldPos)
    {
        UniverseChunk chunk;
        UniverseChunkSector chunkSector = GetChunkSector(worldPos, out chunk);

        // Catched empty chunks
        if (chunkSector == null)
            return false;

        return chunkSector.shipExists;
    }
    #endregion
}