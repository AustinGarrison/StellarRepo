using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UniverseChunkSector
{
    Vector2Int chunk;
    public Vector2Int sectorPosition;
    public int universeSectorX;
    public int universeSectorY;

    public Vector3 spawnPoint;
    public bool shipExists;

    private int randomInt;
    public UniverseChunkSector(int globalChunkX, int globalChunkY, Vector2Int sector, int nSectors)
    {
        this.sectorPosition = sector;

        universeSectorX = sector.x + (globalChunkX * nSectors);
        universeSectorY = sector.y + (globalChunkY * nSectors);

        nLehmer = (uint)((universeSectorX & 0xFFFF) << 16 | (universeSectorY & 0xFFFF));

        randomInt = RandomInt(0, 40);
        shipExists = randomInt == 1;

        if (!shipExists)
        {
            return;
        }
    }

    /// <summary>
    /// Generation
    /// </summary>
    public uint nLehmer = 0;
    private uint Lehmer32()
    {
        //nLehmer += 0xe129fc15;
        nLehmer += 0xe129fc16;
        ulong tmp;
        tmp = (ulong)nLehmer * 0x4a39b70d;
        uint m1 = (uint)((tmp >> 32) ^ tmp);
        tmp = (ulong)m1 * 0x12fad5c9;
        uint m2 = (uint)((tmp >> 32) ^ tmp);

        return m2;
    }

    int RandomInt(int min, int max)
    {
        int randomInt = (int)(Math.Abs(Lehmer32()) % (max - min)) + min;

        return randomInt;
    }

    public string GetIsShip()
    {
        return shipExists.ToString();
    }

    public string GetRandomInt()
    {
        return randomInt.ToString();
    }
}
