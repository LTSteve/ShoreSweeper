using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HullImgHolder : MonoBehaviour, IDropTarget
{
    public void Drop(object o)
    {
        if (o.GetType() != typeof(PurchaseItem.Purchase))
        {
            return;
        }

        var purchase = ((PurchaseItem.Purchase)o);

        if(purchase.type != PurchaseItem.PurchaseType.Hull)
        {
            return;
        }

        BuyView.Instance.AddHullToCart(purchase.index);
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
