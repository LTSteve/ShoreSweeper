using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Transform[] Masts;

    public Vector3 WakeOffset;

    public float BaseSpeed;

    public float ViewDistance;

    public float GetTotalSpeed()
    {
        var speed = BaseSpeed;

        foreach(var mast in Masts)
        {
            var sail = mast.GetComponentInChildren<Sail>();

            if(sail == null)
            {
                continue;
            }

            speed += sail.Speed;
        }

        return speed;
    }
}
