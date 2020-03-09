using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    public static LoadingView Instance;
    public static string FlavorString = "Loading...";
    public static float Doneness = 0f;

    public static void Set(float doneness, string flavor = null)
    {
        FlavorString = flavor == null ? FlavorString : flavor;
        Doneness = doneness;
    }

    public static void Enable()
    {
        if(Instance && Instance.gameObject)
            Instance.gameObject.SetActive(true);
    }

    public Image LoadingBar;
    public SVGImage SpinningBoat;
    public Text FlavorText;

    public float BoatRotationRate = 0.1f;

    private float currentBoatRot;

    private Coroutine closing;

    private float loadingBarMaxWidth;

    private void Start()
    {
        Instance = this;

        loadingBarMaxWidth = LoadingBar.rectTransform.rect.width;
    }

    private void Update()
    {
        if(Doneness >= 1f && closing == null)
        {
            closing = StartCoroutine(_close());
            return;
        }

        currentBoatRot = (currentBoatRot + BoatRotationRate * Time.deltaTime) % 1f;
        SpinningBoat.transform.rotation = Quaternion.Euler(0, 0, currentBoatRot * 360f);

        FlavorText.text = FlavorString;

        LoadingBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Doneness * loadingBarMaxWidth);
    }

    private IEnumerator _close()
    {
        yield return new WaitForSeconds(0.25f);

        Doneness = 0f;
        FlavorString = "Loading...";
        closing = null;
        gameObject.SetActive(false);
    }
}
