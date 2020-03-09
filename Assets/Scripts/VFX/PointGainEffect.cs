using UnityEngine;
using UnityEngine.UI;

public class PointGainEffect : MonoBehaviour
{
    public float Decay = 0.25f;

    public Text MyText;

    private void Update()
    {
        Decay -= Time.deltaTime;

        if(Decay < 0.1f)
        {
            MyText.color = new Color(MyText.color.r, MyText.color.g, MyText.color.b, Decay * 10f);
            transform.position = transform.position + new Vector3(0, Time.deltaTime, 0);
        }
        else
        {
            transform.position = transform.position + new Vector3(0, 0.5f + Time.deltaTime, 0);
        }

        if(Decay <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}