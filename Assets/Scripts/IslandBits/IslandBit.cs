using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBit : MonoBehaviour
{
    public bool Shown = false;
    public int location;

    public ProceduralGenerator Parent;

    public Transform Pop;
    
    public void SetParent(ProceduralGenerator parent)
    {
        Parent = parent;
    }

    public virtual void Show()
    {
        Shown = true;
        return;
    }
}
