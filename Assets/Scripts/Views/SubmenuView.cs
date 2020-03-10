using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmenuView : MonoBehaviour
{
    public bool open = false;

    private Vector3 startingPos;
    private Vector3 zero = new Vector3(10, 0, 0);
    private bool opening = false;

    private bool initialized = false;
    private bool doOpen = false;

    protected void Start()
    {
        startingPos = transform.localPosition;
    }

    public void ToggleOpen()
    {
        if (!opening)
        {
            doOpen = false;
            open = !open;

            opening = true;

            StartCoroutine(_shiftPosition(open));

            ClearView();
        }
    }

    public virtual void ClearView()
    {
        //do something to clear the view
    }

    private IEnumerator _shiftPosition(bool open)
    {
        var timer = 0.2f;

        while(timer > 0)
        {

            if (open)
            {
                transform.localPosition = Vector3.Lerp(startingPos, zero, 1f - (timer / 0.2f));
            }
            else
            {
                transform.localPosition = Vector3.Lerp(zero, startingPos, 1f - (timer / 0.2f));
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        if (open)
        {
            transform.localPosition = zero;
        }
        else
        {
            transform.localPosition = startingPos;
        }

        opening = false;
    }
}
