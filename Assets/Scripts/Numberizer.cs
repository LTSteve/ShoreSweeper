using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class Numberizer
{
    private static int rootSeed;

    public int seed;

    private Random.State state;

    public Numberizer(int seed = 0, bool root = false)
    {
        this.seed = seed;
        if(root)
            rootSeed = this.seed;
        Random.InitState(seed);
        state = Random.state;
    }

    public Numberizer GetNeighbor(int x, int y)
    {
        if(x == 0 && y == 0)
        {
            return this;
        }

        var longseed = (long)seed;
        var longx = (long)x;
        var longy = (long)y;

        var newSeed = (int)(longseed + longx * 65536 + longy);

        return new Numberizer(newSeed);
    }

    public int GetNumeral(int cap = 10)
    {
        Random.state = state;

        var nextNum = Random.Range(0,cap);

        state = Random.state;

        return nextNum % cap;
    }

    public static int Numberize(string seed)
    {
        seed = string.IsNullOrEmpty(seed) ? "0" : seed;

        var chars = seed.ToCharArray();
        var cleaned = new StringBuilder();
        for(var i = 0; i < seed.Length; i++)
        {
            var character = chars[i];
            cleaned.Append(((int)character));
        }

        return int.Parse(cleaned.ToString().Substring(0,9));
    }


    private static Numberizer display;

    public static Numberizer GetDisplayNumbers()
    {
        if(display == null)
        {
            display = new Numberizer(Numberize("Display"));
        }

        return display;
    }
}
