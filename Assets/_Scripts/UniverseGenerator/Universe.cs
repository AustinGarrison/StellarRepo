using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class Universe
{
    public enum MoveChunkDir
    {
        North,
        South,
        East,
        West
    }

    public GameObject visibleShipsTextParent;

    SerializedChunks activeChunksList;
    Vector2Int centerChunkOffset = new Vector2Int(0, 0);

    internal int currentCenterChunkX;
    internal int currentCenterChunkY;

    int sectorGUISize;
    int numberOfSectorsXY;
    int numChunksX, numChunksY;


    public Universe(SerializedChunks activeChunksList, int numberOfSectorsXY, int sectorGUISize, int numChunkX, int numChunkY, SpawnType spawnType)
    {
        this.activeChunksList = activeChunksList;
        this.numberOfSectorsXY = numberOfSectorsXY;
        this.sectorGUISize = sectorGUISize;
        this.numChunksX = numChunkX;
        this.numChunksY = numChunkY;
        type = spawnType;

        currentCenterChunkX = 0;
        currentCenterChunkY = 0;

        visibleShipsTextParent = new GameObject("UniverseTextParent");

        StartingChunks();

        currentCenterChunkX = 0;
        currentCenterChunkY = 0;
    }

    private void StartingChunks()
    {
        if (activeChunksList != null && activeChunksList._activeChunks != null)
        {
            GenerateChunks(true);
        }
    }

    public void MoveChunks(MoveChunkDir newDir)
    {
        visibleShipsTextParent = new GameObject("UniverseTextParent");

        switch (newDir)
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

        GenerateChunks(true);
    }

    private void GenerateChunks(bool startingChunks)
    {
        if (activeChunksList != null && activeChunksList._activeChunks != null)
        {
            for (int localPosX = 0; localPosX < activeChunksList._activeChunks.GetLength(0); localPosX++)
            {
                for (int localPosY = 0; localPosY < activeChunksList._activeChunks.GetLength(1); localPosY++)
                {
                    //UniverseChunk newUniverseChunk = new UniverseChunk

                    activeChunksList._activeChunks[localPosX, localPosY].SetupChunk(

                        // Global Positions
                        //(UInt32)localPosX + currentCenterChunkX,
                        localPosX + currentCenterChunkX,
                        localPosY + currentCenterChunkY,

                        // Local Positions
                        localPosX,
                        localPosY,
                        numberOfSectorsXY,
                        sectorGUISize,
                        out bool chunkHasShip);


                    if (startingChunks && chunkHasShip)
                    {
                        // Render the chunks to the screen
                        ChunkVisuals(activeChunksList._activeChunks[localPosX, localPosY]);
                    }
                }
            }
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
        if ((x < 0 || y < 0) || (x > numChunksX - 1 || y > numChunksY - 1))
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

        GetSectorXY(worldPos, out sectorX, out sectorY);
        UniverseChunkSector chunkSector = GetChunkSector(chunk, sectorX, sectorY);

        return chunkSector;
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
        chunkToRender.chunkTextParent = new GameObject("ChunkTextParent");
        chunkToRender.chunkTextParent.transform.SetParent(visibleShipsTextParent.transform);
        chunkToRender.debugTextArray = new string[numberOfSectorsXY, numberOfSectorsXY];

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



    public SpawnType type;
        
    private void VisualizeSectorShips(UniverseChunk chunk, int x, int y)
    {
        if (chunk.sectorsArray[x, y]?.GetIsShip() == "True")
        {
            chunk.debugTextArray[x, y] = "Ship";

            Vector3 textPosition = chunk.GetSectorPosition(x, y);
            
            textPosition.x -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkX;
            textPosition.y -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkY;//* centerChunkOffset.y;

            chunk.sectorsArray[x, y].spawnPoint = textPosition + new Vector3(sectorGUISize, sectorGUISize, 0) / 2;

            if (type == SpawnType.Both)
            {
                CreateSectorText(
                    chunk.chunkTextParent.transform,
                    chunk.debugTextArray[x, y],
                    chunk.sectorsArray[x, y].spawnPoint);

                SOSShipVisualSpawner.Instance.SpawnAShip(chunk.sectorsArray[x, y], this);

                return;
            }

            if (type == SpawnType.Text)
            {
                CreateSectorText(
                    chunk.chunkTextParent.transform,
                    chunk.debugTextArray[x, y],
                    chunk.sectorsArray[x, y].spawnPoint);
            }

            if(type == SpawnType.Sphere)
            {
            }


        }
        else
        {
            chunk.debugTextArray[x, y] = "";
        }

        chunk.InitSectorChangedEvent();
    }


    private void VisualizeSectorRandomInt(UniverseChunk chunk, int x, int y)
    {

        chunk.debugTextArray[x, y] = chunk.sectorsArray[x, y]?.GetRandomInt();

        Vector3 textPosition = chunk.GetSectorPosition(x, y); 
        
        textPosition.x -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkX;
        textPosition.y -= (numberOfSectorsXY * sectorGUISize) * currentCenterChunkY;//* centerChunkOffset.y;

        CreateSectorText(
                chunk.chunkTextParent.transform,
                chunk.debugTextArray[x, y],
                textPosition + new Vector3(sectorGUISize, sectorGUISize, 0) / 2);

        chunk.InitSectorChangedEvent();
    }

    public static TextMesh CreateSectorText(Transform parent, string text, Vector3 localPosition)
    {
        GameObject gameObject = new GameObject("Sector_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.text = text;
        textMesh.fontSize = 20;
        textMesh.color = Color.white;
        return textMesh;
    }

    private void DrawChunkBorders(UniverseChunk chunk)
    {
        DrawBorder(chunk.GetSectorPosition(0, 0), chunk.GetSectorPosition(0, chunk.sectorsArray.GetLength(1)));
        DrawBorder(chunk.GetSectorPosition(0, 0), chunk.GetSectorPosition(chunk.sectorsArray.GetLength(1), 0));

        // Visible Universe Top
        DrawBorder(chunk.GetSectorPosition(0, chunk.sectorsArray.GetLength(1)), chunk.GetSectorPosition(chunk.sectorsArray.GetLength(0), chunk.sectorsArray.GetLength(1)));
        // Visible Universe Right Side
        DrawBorder(chunk.GetSectorPosition(chunk.sectorsArray.GetLength(0), chunk.sectorsArray.GetLength(1)), chunk.GetSectorPosition(chunk.sectorsArray.GetLength(0), 0));
    }

    private void DrawBorder(Vector3 start, Vector3 end)
    {
        start.y -= (numberOfSectorsXY * sectorGUISize) * centerChunkOffset.y;
        end.y -= (numberOfSectorsXY * sectorGUISize) * centerChunkOffset.y;

        Debug.DrawLine(start, end, Color.white, 100f);
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
