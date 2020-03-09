using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class SaveFile
{
    public ZoneData[] zoneData;
    public float score;
    public int playercoords;
    public ShipData shipData;
    public string playerName;
    public float time = 0f;
    public long lastPlayed = 0;
}
