using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Tile
{
    public Sprite ClearSprite { get; internal set; }

    public override void Activate()
    {
        if (Flaged || Shown || GetComponent<SpriteRenderer>().sprite == ClearSprite)
        {
            return;
        }
        base.Activate();

        Director.PlayerScore = Director.PlayerScore / 2f;

        StartCoroutine(_shakening());

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

            disperseTimer -= Time.deltaTime * speed;
        }

        var mySprite = GetComponent<SpriteRenderer>();
        CoverRenderer.sprite = mySprite.sprite;
        mySprite.sprite = ClearSprite;

        CoverRenderer.transform.rotation = Quaternion.identity;
        CoverRenderer.transform.localScale = new Vector3(1,1,1);
        CoverRenderer.color = Color.white;

        disperseTimer = 0.25f;

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

            disperseTimer -= Time.deltaTime * speed;
        }

        CoverRenderer.sprite = null;
    }

    private IEnumerator _shakening()
    {
        var numbers = Numberizer.GetDisplayNumbers();
        var timer = 0.1f;

        var cameraLocalPos = Camera.main.transform.localPosition;

        while(timer > 0)
        {
            Camera.main.transform.localPosition = cameraLocalPos + new Vector3(numbers.GetNumeral() * 0.07f, numbers.GetNumeral() * 0.01f);

            yield return null;
            timer -= Time.deltaTime;
        }

        Camera.main.transform.localPosition = cameraLocalPos;
    }
}
