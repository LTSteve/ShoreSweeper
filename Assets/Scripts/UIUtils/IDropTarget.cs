using UnityEngine.EventSystems;

public interface IDropTarget: IPointerEnterHandler, IPointerExitHandler
{
    void Drop(object o);
}