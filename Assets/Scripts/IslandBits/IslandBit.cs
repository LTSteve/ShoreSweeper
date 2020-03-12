using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBit : MonoBehaviour
{
    public bool Shown = false;
    public int location;

    public ProceduralGenerator Parent;

    public Transform Pop;

    public Animator myAnimator;

    public void SetParent(ProceduralGenerator parent)
    {
        Parent = parent;
    }

    public virtual void Show()
    {
        if (Shown)
        {
            return;
        }

        Shown = true;

        myAnimator.enabled = true;
    }

    public void Spawned()
    {
        Instantiate(Pop, transform.position, Quaternion.identity);
    }
}
