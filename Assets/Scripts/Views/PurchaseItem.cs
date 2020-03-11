//DEPRECATED

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchaseItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public Text Speed;
    public Text View;
    public Text Cost;
    public SVGImage Image;

    public int hull = -1;
    public int sail = -1;

    public bool entered = false;
    public bool dragging = false;

    private float cost;
    

    public void SetHull(int hull, Sprite sprite)
    {
        var hullData = Director.D.GameData.hull[hull];

        Speed.text = "Speed: " + hullData.baseSpeed;
        View.text = "View: " + hullData.viewDistance;
        Cost.text = "$ " + hullData.cost;

        Image.sprite = sprite;
        var bounds = sprite.bounds.size;
        //Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64f * (bounds.y / bounds.x));
        Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.y * 64f);
        Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.x * 64f);
        Image.transform.rotation = Quaternion.Euler(0, 0, 90f);

        this.cost = hullData.cost;
        this.hull = hull;
    }

    public void SetSail(int sail, Sprite sprite)
    {
        var sailData = Director.D.GameData.sail[sail];

        Speed.text = "Speed: " + sailData.speed;
        View.text = "";
        Cost.text = "$ " + sailData.cost;

        Image.sprite = sprite;
        var bounds = sprite.bounds.size;
        //Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(64f * (bounds.y / bounds.x), 32f, float.MaxValue));
        Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.y * 64f * 5f);
        Image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.x * 64f * 5f);
        if(bounds.x < bounds.y)
            Image.transform.rotation = Quaternion.Euler(0, 0, 90f);

        this.cost = sailData.cost;
        this.sail = sail;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorBro.Do(8);
        entered = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!entered)
        {
            return;
        }
        dragging = true;

        CursorBro.Do(7);

        HeldItem.Hold(new Purchase
        {
            type = hull >= 0 ? PurchaseType.Hull : PurchaseType.Sail,
            cost = cost,
            index = hull >= 0 ? hull : sail
        }, Image.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        entered = false;

        if (!dragging)
        {
            CursorBro.Do(0);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public class Purchase
    {
        public PurchaseType type;
        public float cost;
        public int index;
    }

    public enum PurchaseType
    {
        Hull,
        Sail
    }
}