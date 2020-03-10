using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public int Index = 0;

    public void Click()
    {
        transform.parent.parent.parent.parent.GetComponent<LoadGameView>().SelectRow(this);
    }
}
