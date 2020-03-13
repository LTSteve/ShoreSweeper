using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarterWindowView : MonoBehaviour
{
    public static BarterWindowView Instance;

    public Text StatPrefab;
    public Sprite[] HullSprites;
    public Sprite[] SailSprites;
    public Color PositiveColor, NegativeColor;

    public Vector3 zero = Vector3.zero;
    private Vector3 startingPos;

    private bool open = false;
    private bool shifting = false;

    public static List<SailData> SailsToSell = new List<SailData>();

    public static int SailSaleNum = 0;
    public static int CurrentlySellingNum = 0;

    public static float Cost = 0;

    public static bool IsHull;
    public static int ItemNumber;

    void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        startingPos = transform.localPosition;
    }

    public static void Init(int ItemNumber, bool IsHull)
    {

        if (Instance.shifting)
        {
            return;
        }

        BarterWindowView.ItemNumber = ItemNumber;
        BarterWindowView.IsHull = IsHull;

        var transform = Instance.transform;

        var playerData = PlayerController.Instance;
        var gameData = Director.D.GameData;
        var playerHull = gameData.hull[playerData.hull];
        var playerSails = playerData.sails;

        var stat1 = transform.Find("Trade View/Offer/Item/Item Statblock/Stat 1").GetComponent<Text>();
        var stat2 = transform.Find("Trade View/Offer/Item/Item Statblock/Stat 2").GetComponent<Text>();
        var stat3 = transform.Find("Trade View/Offer/Item/Item Statblock/Stat 3").GetComponent<Text>();

        if (IsHull)
        {
            var hull = gameData.hull[ItemNumber];

            SailsToSell.Clear();

            var mastCountDiff = hull.mastCount - playerHull.mastCount;

            SailSaleNum = 0;

            if (mastCountDiff < 0)
            {
                for (var i = playerHull.mastCount + mastCountDiff; i < playerHull.mastCount; i++)
                {
                    SailsToSell.Add(gameData.sail[playerSails[i]]);
                }

                SailSaleNum = -mastCountDiff;
            }

            transform.Find("Trade View/Offer/Item/Item Name").GetComponent<Text>().text = hull.name;

            transform.Find("Trade View/Offer/Item/Item Image/Image").GetComponent<SVGImage>().sprite = Instance.HullSprites[ItemNumber];

            stat1.text = "Speed: " + hull.baseSpeed;
            stat2.text = "View: " + hull.viewDistance;
            stat2.gameObject.SetActive(true);
            stat3.text = "Masts: " + hull.mastCount;
            stat3.gameObject.SetActive(true);


            var statBlock = transform.Find("Trade View/Diff/StatBlock");

            foreach (Transform child in statBlock)
            {
                Destroy(child.gameObject);
            }

            //calculate speed change
            var playerSpeed = playerHull.baseSpeed;
            var newSpeed = hull.baseSpeed;
            foreach (var sailNum in playerSails)
            {
                var sail = gameData.sail[sailNum];

                playerSpeed += sail.speed;

                if (SailsToSell.Contains(sail))
                {
                    continue;
                }

                newSpeed += sail.speed;
            }

            if (newSpeed != playerSpeed)
            {
                var speedStatDisplay = Instantiate(Instance.StatPrefab, statBlock);

                var deltaSpeed = newSpeed - playerSpeed;

                speedStatDisplay.text = "Speed: " + (deltaSpeed > 0 ? "+" : "-") + Mathf.Abs(deltaSpeed);
                speedStatDisplay.color = deltaSpeed > 0 ? Instance.PositiveColor : Instance.NegativeColor;
            }

            //calculate view change
            var deltaView = hull.viewDistance - playerHull.viewDistance;

            if (deltaView != 0)
            {
                var viewStatDisplay = Instantiate(Instance.StatPrefab, statBlock);

                viewStatDisplay.text = "View Range: " + (deltaView > 0 ? "+" : "-") + Mathf.Abs(deltaView);
                viewStatDisplay.color = deltaView > 0 ? Instance.PositiveColor : Instance.NegativeColor;
            }

            //calculate masts change
            var deltaMasts = hull.mastCount - playerHull.mastCount;

            if (deltaMasts != 0)
            {
                var mastStatDisplay = Instantiate(Instance.StatPrefab, statBlock);

                mastStatDisplay.text = "Masts: " + (deltaMasts > 0 ? "+" : "-") + Mathf.Abs(deltaMasts);
                mastStatDisplay.color = deltaMasts > 0 ? Instance.PositiveColor : Instance.NegativeColor;
            }

            //calculate cost (sellbacks are worth 25% the value)
            Cost = -hull.cost;
            foreach (var sail in SailsToSell)
            {
                Cost += sail.cost * 0.25f;
            }
            Cost += playerHull.cost * 0.25f;

            var moneyText = transform.Find("Trade View/Cost/Money").GetComponent<Text>();

            moneyText.text = (Cost < 0 ? "-" : "") + "$" + Mathf.Abs(Cost);
            moneyText.color = (Cost < 0 ? Instance.NegativeColor : Instance.PositiveColor);

            //hide sails to lose if there are no sails to lose
            if(mastCountDiff >= 0)
            {
                transform.Find("Trade View/Cost/Sails to lose").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("Trade View/Cost/Sails to lose").gameObject.SetActive(true);

                //update sails to lose label
                transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().text = "Sail" + (mastCountDiff < -1 ? "s" : "") + " To Give";
                transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().color = Instance.PositiveColor;

                //select all 'lost' sails
                for (var i = 0; i < 3; i++)
                {
                    var sailHolder = transform.Find("Trade View/Cost/Sails to lose/Sail" + (i + 1));

                    if (playerHull.mastCount <= i)//no more masts to check
                    {
                        sailHolder.gameObject.SetActive(false);
                        continue;
                    }

                    sailHolder.gameObject.SetActive(true);

                    sailHolder.GetComponent<Image>().enabled = SailsToSell.Contains(gameData.sail[playerSails[i]]); //show background for active sails to sell

                    sailHolder.Find("Image").GetComponent<SVGImage>().sprite = Instance.SailSprites[playerSails[i]];
                }
            }

            CurrentlySellingNum = SailSaleNum;
        }
        else
        {
            var sail = gameData.sail[ItemNumber];

            SailData sailToSell = null;

            SailSaleNum = 1;

            foreach (var sailNum in playerSails)
            {
                var sellableSail = gameData.sail[sailNum];

                if(sellableSail == null)
                {
                    sailToSell = null;
                    SailSaleNum = 0;
                    break;
                }

                if (sailToSell == null || sellableSail.cost <= sailToSell.cost)
                {
                    sailToSell = sellableSail;
                }
            }

            transform.Find("Trade View/Offer/Item/Item Name").GetComponent<Text>().text = sail.name;

            transform.Find("Trade View/Offer/Item/Item Image/Image").GetComponent<SVGImage>().sprite = Instance.SailSprites[ItemNumber];

            stat1.text = "Speed: " + sail.speed;
            stat2.gameObject.SetActive(false);
            stat3.gameObject.SetActive(false);

            var statBlock = transform.Find("Trade View/Diff/StatBlock");

            foreach (Transform child in statBlock)
            {
                Destroy(child.gameObject);
            }

            //calculate speed change
            var deltaSpeed = sail.speed - (sailToSell == null ? 0 : sailToSell.speed);

            if (deltaSpeed != 0)
            {
                var speedStatDisplay = Instantiate(Instance.StatPrefab, statBlock);

                speedStatDisplay.text = "Speed: " + (deltaSpeed > 0 ? "+" : "-") + Mathf.Abs(deltaSpeed);
                speedStatDisplay.color = deltaSpeed > 0 ? Instance.PositiveColor : Instance.NegativeColor;
            }

            //calculate cost (sellbacks are worth 25% the value)
            Cost = (sailToSell == null ? 0 : (sailToSell.cost * 0.25f)) - sail.cost;

            var moneyText = transform.Find("Trade View/Cost/Money").GetComponent<Text>();

            moneyText.text = (Cost < 0 ? "-" : "") + "$" + Mathf.Abs(Cost);
            moneyText.color = (Cost < 0 ? Instance.NegativeColor : Instance.PositiveColor);

            //display trade info if a sail is to be sold
            if (sailToSell == null)
            {
                transform.Find("Trade View/Cost/Sails to lose").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("Trade View/Cost/Sails to lose").gameObject.SetActive(true);

                //update sails to lose label
                transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().text = "Sail To Give";
                transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().color = Instance.PositiveColor;

                //select 'lost'
                for (var i = 0; i < 3; i++)
                {
                    var sailHolder = transform.Find("Trade View/Cost/Sails to lose/Sail" + (i + 1));

                    if (playerHull.mastCount <= i)//no more masts to check
                    {
                        sailHolder.gameObject.SetActive(false);
                        continue;
                    }

                    sailHolder.gameObject.SetActive(true);

                    sailHolder.GetComponent<Image>().enabled = sailToSell == gameData.sail[playerSails[i]]; //show background for active sails to sell

                    sailHolder.Find("Image").GetComponent<SVGImage>().sprite = Instance.SailSprites[playerSails[i]];

                    SailsToSell.Clear();
                    SailsToSell.Add(sailToSell);
                }
            }

            CurrentlySellingNum = SailSaleNum;
        }

        Instance.OpenMenu();
    }

    public void SelectSail(int number)
    {
        var player = PlayerController.Instance;
        if (player == null) return;

        var sail = player.sails[number-1];

        if(sail < 0)
        {
            return;
        }

        var sailHolder = transform.Find("Trade View/Cost/Sails to lose/Sail" + number);
        var sailHolderImage = sailHolder.GetComponent<Image>();

        if (sailHolderImage.enabled) //deselect
        {
            CurrentlySellingNum--;

            SailsToSell.Remove(Director.D.GameData.sail[sail]);

            sailHolderImage.enabled = false;
        }
        else //select
        {
            CurrentlySellingNum++;

            SailsToSell.Add(Director.D.GameData.sail[sail]);

            sailHolderImage.enabled = true;
        }

        if(CurrentlySellingNum == SailSaleNum)
        {
            //update sails to lose label
            transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().color = PositiveColor;
        }
        else
        {
            transform.Find("Trade View/Cost/Sails to lose/Text").GetComponent<Text>().color = NegativeColor;
        }
    }

    public void OpenMenu()
    {
        open = true;
        shifting = true;
        StartCoroutine(_shiftPosition(open));
    }

    public void CloseMenu()
    {
        if (open != true || shifting)
            return;

        if (PlayerController.Instance && PlayerController.Instance.CurrentInteraction != null)
        {
            PlayerController.Instance.CurrentInteraction.CloseInteraction();
        }

        open = false;
        shifting = true;
        StartCoroutine(_shiftPosition(open));
    }
    
    public void Accept()
    {
        if(Director.PlayerScore < Cost || CurrentlySellingNum != SailSaleNum)
        {
            //Tell player
            return;
        }

        Director.PlayerScore -= Cost;

        var player = PlayerController.Instance;
        var gameData = Director.D.GameData;

        if (IsHull)
        {
            var hull = gameData.hull[ItemNumber];

            var newSails = hull.defaultSails;

            var actualIndex = 0;
            for(var i = 0; i < player.sails.Length; i++)
            {
                var sail = gameData.sail[player.sails[i]];

                if (SailsToSell.Contains(sail))
                {
                    SailsToSell.Remove(sail);
                    continue;
                }

                newSails[actualIndex] = player.sails[i];
                actualIndex++;
            }
            
            player.SetHull(ItemNumber, false);
            player.SetSails(newSails);
        }
        else
        {
            var playerSails = player.sails;

            for(var i = 0; i < playerSails.Length; i++)
            {
                if (gameData.sail[playerSails[i]] == SailsToSell[0])
                {
                    playerSails[i] = ItemNumber;
                    player.SetSails(playerSails);
                    break;
                }
            }
        }

        CloseMenu();
    }

    private IEnumerator _shiftPosition(bool to)
    {
        var timer = 0.2f;

        while (timer > 0)
        {

            if (open)
            {
                transform.localPosition = Vector3.Lerp(startingPos, zero, 1f - (timer / 0.2f));
            }
            else
            {
                transform.localPosition = Vector3.Lerp(zero, startingPos, 1f - (timer / 0.2f));
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        if (open)
        {
            transform.localPosition = zero;
        }
        else
        {
            transform.localPosition = startingPos;
        }
        shifting = false;
    }
}
