using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZoneData
{
    public bool cleared = false;

    public int coordinates = 0;

    public bool honked;

    public List<int> ClearedTiles;
    public List<int> FlaggedTiles;

    public ZoneData(int coords, bool cleared = false)
    {
        coordinates = coords;
        this.cleared = cleared;
    }

    public bool IsCleared(int toCheck)
    {
        if (cleared) return true;

        if (ClearedTiles == null)
        {
            return false;
        }

        return ClearedTiles.IndexOf(toCheck) >= 0;
    }
    public bool IsFlagged(int toCheck)
    {
        if (FlaggedTiles == null)
        {
            return false;
        }

        return FlaggedTiles.IndexOf(toCheck) > 0;
    }

    public void SaveFlagged(int toFlag)
    {
        if (cleared) return;

        if (FlaggedTiles == null)
        {
            FlaggedTiles = new List<int>();
        }

        if (IsFlagged(toFlag))
        {
            FlaggedTiles.Remove(toFlag);
        }
        else
        {
            FlaggedTiles.Add(toFlag);
        }

        Director.Save();
    }

    public void SaveCleared(int toClear)
    {
        if (cleared) return;

        if (ClearedTiles == null)
        {
            ClearedTiles = new List<int>();
        }

        ClearedTiles.Add(toClear);

        Director.Save();
    }

    public void ClearAll()
    {
        cleared = true;
        ClearedTiles = null;
        FlaggedTiles = null;

        Director.Save();
    }
}