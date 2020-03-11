using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBit : IslandBit
{
    public Animator myAnimator;

    private float countdown;
    private bool countingDown = false;

    public override void Show()
    {
        if (Shown)
        {
            return;
        }

        base.Show();

        myAnimator.enabled = true;
    }

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

    public void TreeSpawned()
    {
        Instantiate(Pop, transform.position, Quaternion.identity);

        countdown = Numberizer.GetDisplayNumbers().GetNumeral(100) * 0.01f + 0.01f;
        countingDown = true;
    }
}
