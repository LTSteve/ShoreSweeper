using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyView : MonoBehaviour
{
    public static BuyView Instance;

    public Sprite[] hullSprites;
    public Sprite[] sailSprites;

    public PurchaseItem ItemPrefab;
    public SVGImage[] SailImages;
    public SVGImage HullImg;
    public PlayerController Player;
    public Text Cost;

    public Transform HullPurchaseWindow;
    public Transform SailPurchaseWindow;

    public bool open = false;

    private Vector3 startingPos;
    private Vector3 zero = new Vector3(10, 10, 0);
    private bool opening = false;

    private bool initialized = false;
    private bool doOpen = false;

    void Start()
    {
        Instance = this;
        startingPos = transform.position;
    }

    private void _init()
    {
        var gamedata = Director.D.GameData;

        if(gamedata.hull.Length <= 0)
        {
            return;
        }

        for (var i = 0; i < gamedata.hull.Length; i++)
        {
            var hull = gamedata.hull[i];

            var hullGO = Instantiate(ItemPrefab, HullPurchaseWindow);

            hullGO.SetHull(i, hullSprites[i]);
        }

        for (var i = 0; i < gamedata.sail.Length; i++)
        {
            var sail = gamedata.sail[i];

            var sailGO = Instantiate(ItemPrefab, SailPurchaseWindow);

            sailGO.SetSail(i, sailSprites[i]);
        }

        _toggleSailSlots(gamedata.hull[Player.hull]);

        initialized = true;
    }

    void Update ()
    {
        if (!initialized)
        {
            _init();
            if (!initialized) return;
        }

        if ((Director.D != null && Director.D.Loaded && Input.GetKeyDown(KeyCode.B)) || doOpen)
        {
            ToggleOpen();
        }
    }

    private int hullInCart = -1;
    private List<int> sailsInCart = new List<int>();
    private float totalCost = 0f;

    public void ToggleOpen()
    {
        if (!opening)
        {
            doOpen = false;
            open = !open;

            opening = true;

            StartCoroutine(_shiftPosition(open));

            if (open)
            {
                _setHullImg(Player.hull);
                _toggleSailSlots(Director.D.GameData.hull[Player.hull]);
                _setSailImgs(Player.sails);
            }
        }
    }

    public void AddHullToCart(int hull)
    {
        hullInCart = hull;

        _setHullImg(hull);

        var hullData = Director.D.GameData.hull[hull];

        _toggleSailSlots(hullData);

        while (sailsInCart.Count > hullData.mastCount)
        {
            sailsInCart.RemoveAt(sailsInCart.Count - 1);
        }

        _recalculateCost();
    }

    public void AddSailToCart(int sail, int mast)
    {
        if(sailsInCart.Count <= mast)
        {
            for(var i = sailsInCart.Count; i <= mast; i++)
            {
                sailsInCart.Add(Player.sails.Length > i ? Player.sails[i] : -1);
            }
        }

        sailsInCart[mast] = sail;

        _setSailImgs(sailsInCart.ToArray());

        _recalculateCost();
    }

    private void _recalculateCost()
    {
        var cost = 0f;

        if(hullInCart != -1)
        {
            if(hullInCart != Player.hull)
            {
                cost += Director.D.GameData.hull[hullInCart].cost;
            }
        }

        if(sailsInCart.Count > 0)
        {
            for(var i = 0; i < sailsInCart.Count; i++)
            {
                var sail = sailsInCart[i];

                if(sail == -1)
                {
                    continue;
                }

                if(Player.sails.Length > i && Player.sails[i] == sail)
                {
                    continue;
                }

                cost += Director.D.GameData.sail[sail].cost;
            }
        }

        totalCost = cost;

        Cost.text = "Cost: " + totalCost;
    }

    public void MakePurchase()
    {
        if(Director.PlayerScore < totalCost)
        {
            return;
        }

        var reductedCost = 0f;

        if (hullInCart >= 0 && hullInCart != Player.hull)
        {
            reductedCost = Director.D.GameData.hull[hullInCart].cost;
            Director.PlayerScore -= reductedCost;
        }

        Player.SetHull(hullInCart < 0 ? Player.hull : hullInCart, false);

        var checksout = sailsInCart.Count > 0;
        for(var i = 0; i < sailsInCart.Count; i++)
        {
            if(sailsInCart[i] == -1)
            {
                checksout = false;
            }
        }

        if (checksout)
        {
            Player.SetSails(sailsInCart.ToArray());

            Director.PlayerScore -= (totalCost - reductedCost);
        }
        else
        {
            Player.SetSails(Player.sails, true);
        }

        Cost.text = "Cost: 0";

        doOpen = true;
    }

    public void ClearPurchase()
    {
        hullInCart = -1;

        _setHullImg(Player.hull);

        var hullData = Director.D.GameData.hull[Player.hull];

        _toggleSailSlots(hullData);

        for (var i = 0; i < Player.sails.Length; i++)
        {
            SailImages[i].sprite = sailSprites[i];
        }

        sailsInCart.Clear();

        _recalculateCost();
    }

    private IEnumerator _shiftPosition(bool open)
    {
        var timer = 0.2f;

        while(timer > 0)
        {

            if (open)
            {
                transform.position = Vector3.Lerp(startingPos, zero, 1f - (timer / 0.2f));
            }
            else
            {
                transform.position = Vector3.Lerp(zero, startingPos, 1f - (timer / 0.2f));
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        if (open)
        {
            transform.position = zero;
        }
        else
        {
            transform.position = startingPos;
        }

        opening = false;
    }

    private void _setHullImg(int hullType)
    {
        var hullData = Director.D.GameData.hull[hullType];

        HullImg.sprite = hullSprites[hullType];
        var bounds = HullImg.sprite.bounds.size;
        HullImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.y * 64f * 1.5f);
        HullImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.x * 64f * 1.5f);
    }

    private void _setSailImgs(int[] sailTypes)
    {
        for (var i = 0; i < 3; i++)
        {
            if(i < sailTypes.Length && sailTypes[i] != -1)
            {
                SailImages[i].sprite = sailSprites[sailTypes[i]];
                var bounds = SailImages[i].sprite.bounds.size;
                SailImages[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.y * 64f * 3f);
                SailImages[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.x * 64f * 3f);
            }
            else
            {
                SailImages[i].sprite = null;
            }
        }
    }

    private void _toggleSailSlots(HullData hullData)
    {
        for (var i = 0; i < 3; i++)
        {
            var sailImg = SailImages[i];
            if (i < hullData.mastCount)
            {
                sailImg.color = Color.white;
                sailImg.GetComponentInParent<SailImgHolder>().disabled = false;
            }
            else
            {
                sailImg.sprite = null;
                sailImg.color = Color.black;
                sailImg.GetComponentInParent<SailImgHolder>().disabled = true;
            }
        }
    }
}
