using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBit : IslandBit
{
    private float countdown;
    private bool countingDown = false;

    private void Update()
    {
        if (countdown <= 0)
        {
            if (countingDown)
            {
                countingDown = false;
                myAnimator.SetTrigger("Wind");
            }
            return;
        }

        countdown -= Time.deltaTime;
    }

    public new void Spawned()
    {
        base.Spawned();

        countdown = Numberizer.GetDisplayNumbers().GetNumeral(100) * 0.01f + 0.01f;
        countingDown = true;
    }
}
