using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : Tile
{
    public GameObject NumberRef;
    public int number = 0;

    public void Start()
    {
        if (NumberRef != null)
        {
            NumberRef.SetActive(Shown);
        }
    }

    public override void Activate()
    {
        if (Flaged || Shown)
        {
            return;
        }
        base.Activate();

        if (NumberRef != null)
        {
            NumberRef.SetActive(true);
        }

        givePoints(number);

        if (Parent)
        {
            Parent.CheckIfIWon();
        }
    }

    public override void Clear()
    {
        StartCoroutine(Disperse());
    }

    private IEnumerator Disperse()
    {
        SpriteRenderer numberRenderer = null;
        if (NumberRef != null)
        {
            numberRenderer = NumberRef.GetComponent<SpriteRenderer>();
        }
        var disperseTimer = 0.25f;
        var displayNumberizer = Numberizer.GetDisplayNumbers();
        var clockwise = displayNumberizer.GetNumeral(2) == 0 ? 1 : -1;
        var speed = displayNumberizer.GetNumeral() * 0.1f + 1f;
        var rotationAmount = displayNumberizer.GetNumeral(15) + 30f;
        var sprite = GetComponent<SpriteRenderer>();

        while (disperseTimer > 0f)
        {
            yield return null;

            var rotation = Quaternion.Euler(0, 0, rotationAmount * clockwise * (0.25f - disperseTimer));

            var progress = 1f - (disperseTimer / 0.25f);
            var scale = Mathf.Pow(1f - progress, 2f) + progress;
            var localScale = new Vector3(scale, scale, scale);
            var color = disperseTimer <= 0.1f ? new Color(1, 1, 1, disperseTimer * 10f) : Color.white;

            CoverRenderer.transform.rotation = rotation;
            CoverRenderer.transform.localScale = localScale;
            CoverRenderer.color = color;

            if (numberRenderer != null)
            {
                numberRenderer.transform.rotation = rotation;
                numberRenderer.transform.localScale = localScale;
                numberRenderer.color = color;
            }

            disperseTimer -= Time.deltaTime * speed;
        }

        CoverRenderer.sprite = null;
        Destroy(numberRenderer.gameObject);
    }
}
