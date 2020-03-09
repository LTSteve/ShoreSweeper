using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ShipData
{
    public int hull;
    public int[] sails;

    public static ShipData StarterPackage = new ShipData
    {
        hull = 0,
        sails = new int[] { 0 }
    };
}
