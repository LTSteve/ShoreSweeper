using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SailImgHolder : MonoBehaviour, IDropTarget
{
    public int mast;
    public bool disabled = false;

    public void Drop(object o)
    {
        if(disabled)
        {
            return;
        }

        if(o.GetType() != typeof(PurchaseItem.Purchase))
        {
            return;
        }

        var purchase = (PurchaseItem.Purchase)o;

        if(purchase.type != PurchaseItem.PurchaseType.Sail)
        {
            return;
        }

        BuyView.Instance.AddSailToCart(purchase.index, mast);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HeldItem.Floating(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HeldItem.UnFloating(this);
    }
}
