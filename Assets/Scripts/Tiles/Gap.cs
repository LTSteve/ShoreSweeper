using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gap : Tile {

    public override void Activate()
    {
        if (Flaged || Shown)
        {
            return;
        }
        
        if (Parent == null)
        {
            return;
        }

        Parent.zoneData.SaveCleared(location);

        StartCoroutine(Disperse());

        Parent.UnfoldNeighborTilesOf(location);
    }

    public override void Clear()
    {
        StartCoroutine(Disperse());
    }

    private IEnumerator Disperse()
    {
        destroying = true;

        var disperseTimer = 0.25f;
        var displayNumberizer = Numberizer.GetDisplayNumbers();
        var clockwise = displayNumberizer.GetNumeral(2) == 0 ? 1 : -1;
        var speed = displayNumberizer.GetNumeral() * 0.1f + 1f;
        var rotationAmount = displayNumberizer.GetNumeral(15) + 30f;
        var sprite = GetComponent<SpriteRenderer>();

        while (disperseTimer > 0f)
        {
            yield return null;

            transform.rotation = Quaternion.Euler(0, 0, rotationAmount * clockwise * (0.25f - disperseTimer));

            var progress = 1f - (disperseTimer / 0.25f);
            var scale = Mathf.Pow(1f - progress, 2f) + progress;
            transform.localScale = new Vector3(scale, scale, scale);

            if(disperseTimer <= 0.1f)
            {
                sprite.color = new Color(1, 1, 1, disperseTimer * 10f);
            }

            disperseTimer -= Time.deltaTime*speed;
        }

        Destroy(this.gameObject);
    }
}
