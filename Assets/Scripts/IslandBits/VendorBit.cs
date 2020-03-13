using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorBit : IslandBit
{
    public int Level = 0;

    private int ItemNumber = 0;
    private bool IsHull = false;

    private bool interactionOpen = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Shown)
        {
            return;
        }

        if (PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.AddInteraction(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!Shown)
        {
            return;
        }

        if(PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.RemoveInteraction(this);
    }

    public override void Show()
    {
        var displayNumbers = Numberizer.GetDisplayNumbers();

        IsHull = displayNumbers.GetNumeral(2) == 0;

        var seed = displayNumbers.GetNumeral() * 0.1f;

        var numHulls = Director.D.GameData.hull.Length;
        var numSails = Director.D.GameData.sail.Length;

        var min = 0;
        if(Level > 2)
        {
            min = 1;
        }
        if(Level > 5)
        {
            min = 3;
        }

        var max = 1;
        if(Level > 1)
        {
            max = 2;
        }
        if(Level > 3)
        {
            max = 5;
        }

        if (IsHull) //5 hulls
        {
            ItemNumber = Mathf.Clamp((int)(numHulls * seed + 0.1f), min, Mathf.Clamp(max, 0, 4));
        }
        else//it's a sail, 6 sails
        {
            ItemNumber = Mathf.Clamp((int)(numSails * seed + 0.1f), min, Mathf.Clamp(max, 0, 5));
        }

        base.Show();
    }

    public void OpenInteraction()
    {
        if (interactionOpen)
        {
            return;
        }

        BarterWindowView.Init(ItemNumber, IsHull);

        interactionOpen = true;
    }

    public void CloseInteraction()
    {
        if (!interactionOpen)
        {
            return;
        }

        interactionOpen = false;
    }
}
