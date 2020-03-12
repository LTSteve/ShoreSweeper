//DEPRECATED (partially, the number stuff is fine but the drag & drop is deprecated)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeldItem : MonoBehaviour
{
    public static HeldItem Instance;

    public SVGImage Display;

    public PointGainEffect PointGainEffectPrefab;
    public static Vector3 PointGainEffectOffset = new Vector3(0f, 1f, 0f);

    private object held;
    private IDropTarget target;

    public void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if(held != null)
        {
            transform.position = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                CursorBro.Do(0);

                if(target != null && held != null)
                {
                    target.Drop(held);
                }

                held = target = null;
                gameObject.SetActive(false);
            }
        }
    }

    public static void Hold(object item, Sprite display)
    {
        Instance.gameObject.SetActive(true);
        Instance.transform.position = Input.mousePosition;
        Instance.held = item;
        Instance.Display.sprite = display;
    }

    public static void Floating(IDropTarget over)
    {
        if (!Instance.gameObject.activeSelf)
        {
            return;
        }
        Instance.target = over;
    }

    public static void UnFloating(IDropTarget over)
    {
        if (!Instance.gameObject.activeSelf)
        {
            return;
        }
        if (Instance.target == over)
        {
            Instance.target = null;
        }
    }

    public static void SpawnPointGain(int toDisplay)
    {
        var numbers = Numberizer.GetDisplayNumbers();
        var xoffset = numbers.GetNumeral() * 0.1f - 0.5f;
        var yoffset = numbers.GetNumeral() * 0.1f;
        var boffset = numbers.GetNumeral() * 0.05f + 0.25f;

        var gain = Instantiate(Instance.PointGainEffectPrefab, Input.mousePosition + PointGainEffectOffset + new Vector3(xoffset, yoffset)*3f, Quaternion.identity, Instance.GetComponentInParent<Canvas>().transform);
        gain.MyText.text = (toDisplay > 0 ? "+" : "") + toDisplay;
        gain.MyText.color = new Color((xoffset+0.5f)/2f + 0.5f, yoffset/2f + 0.5f, boffset);
    }
}
