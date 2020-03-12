using UnityEngine;
using UnityEngine.EventSystems;

public class HoverFinger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int NormalFinger = 0;
    public int SpecialFinger = 6;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorBro.Do(SpecialFinger);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorBro.Do(NormalFinger);
    }
}
