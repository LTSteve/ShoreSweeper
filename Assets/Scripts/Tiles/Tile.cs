using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Shown = false;
    public bool Flaged = false;
    public int location;
    public bool destroying = false;

    public SpriteRenderer CoverRenderer;
    public ProceduralGenerator Parent;

    private Sprite CoverSprite;
    
    public void SetParent(ProceduralGenerator parent)
    {
        Parent = parent;
    }

    public virtual void Activate()
    {
        if (Flaged)
        {
            return;
        }
        if (!Shown)
        {
            CoverRenderer.sprite = null;
            Shown = true;

            if (Parent != null)
            {
                Parent.zoneData.SaveCleared(location);
            }
        }
    }

    public void Flagify()
    {
        if (Shown)
        {
            return;
        }
        if (Flaged)
        {
            CoverRenderer.sprite = CoverSprite;
            Flaged = false;
            if (Parent)
            {
                Parent.CheckIfIWon();
            }
            return;
        }
        CoverSprite = CoverRenderer.sprite;
        CoverRenderer.sprite = PlayerController.FlagSprite;
        Flaged = true;
        if (Parent)
        {
            Parent.CheckIfIWon();
        }
    }

    public virtual void Clear()
    {
        return;
    }

    protected void givePoints(int number = 0)
    {
        if(Parent == null)
        {
            return;
        }

        var points = Parent.CurrentDifficulty * 2 + 5 + number;

        HeldItem.SpawnPointGain(points);

        Director.PlayerScore += points;
    }
}
