using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckBit : IslandBit
{
    private void Start()
    {
        transform.Find("wreck").rotation = Quaternion.Euler(new Vector3(0f, 0f, 360f * (Numberizer.GetDisplayNumbers().GetNumeral(360) / 360f)));
    }
}
