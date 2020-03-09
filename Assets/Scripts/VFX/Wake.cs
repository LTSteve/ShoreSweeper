using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wake : MonoBehaviour {

    public float decayTime = 2f;

    public bool floatAway = true;

    private SpriteRenderer myRenderer;

    private Vector3 startingPosition;

    public float ExtraForce = 0f;

    public void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();

        if (floatAway)
        {
            transform.SetParent(null, true);
        }

        startingPosition = transform.position;
    }

    public void Update()
    {
        decayTime -= Time.deltaTime;

        if (floatAway)
        {
            transform.position = startingPosition + (Vector3)(transform.localToWorldMatrix * new Vector3(-0.25f - ExtraForce, 0, 0) * (2f - decayTime));
        }
        else
        {
            var scaler = 1f + decayTime * .25f;
            transform.localScale = new Vector3(scaler, scaler, scaler);
        }

        if (decayTime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (decayTime<= 1f)
        {
            myRenderer.color = new Color(1, 1, 1, decayTime);
        }
    }
}
