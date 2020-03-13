using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGenerator : MonoBehaviour {

    public Tile MinePrefab;
    public Tile LandPrefab;
    public Tile GapPrefab;
    public GameObject NumberPrefab;
    public TreeBit TreePrefab;
    public WreckBit WreckPrefab;
    public VendorBit VendorPrefab;

    public Sprite[] LandSprites;
    public Sprite[] NumberSprites;
    public Sprite MineSprite;

    public Transform LandRoot;
    public Transform SpritesRoot;

    public Vector3 IslandCenterOffset;

    public int CurrentDifficulty;
    private int MineCount;
    public int Type;
    public bool Honked;

    private Numberizer coordinates;

    private List<Tile> tiles = new List<Tile>();
    private List<IslandBit> bits = new List<IslandBit>();
    private Tile[,] tileMap = new Tile[20, 20];

    public ZoneData zoneData;

    public void Update()
    {
        if (Honked)
        {
            return;
        }

        if(PlayerController.Instance && Vector3.Distance(PlayerController.Instance.transform.position, transform.position + IslandCenterOffset) < 10f + Camera.main.orthographicSize)
        {
            zoneData.honked = Honked = true;

            Honk();
        }
    }

    public IEnumerator Activate(Numberizer coordinates, ZoneData data, bool rootZone = false)
    {
        zoneData = data;
        var completelyCleared = data.cleared;

        CurrentDifficulty = coordinates.GetNumeral() + 1;
        MineCount = (int)(Mathf.Pow(CurrentDifficulty, 2f)/1.1f) + 40;

        Honked = data.honked;
        
        IslandCenterOffset = new Vector3((coordinates.GetNumeral(11) - 5f) * 6f, (coordinates.GetNumeral(11) - 5f) * 6f);

        Type = coordinates.GetNumeral();

        var hasWreck = coordinates.GetNumeral(2) == 0;

        var hasVendor = true;// rootZone ? true : coordinates.GetNumeral(4) == 0;

        var wreckTileType = coordinates.GetNumeral(8);

        var vendorTileType = ((wreckTileType + 1) % 8);
        if (wreckTileType >= 4)
        {
            wreckTileType++;
        }

        if(vendorTileType >= 4)
        {
            vendorTileType++;
        }

        var availableSpots = new List<int>(400);
        for (var i = 0; i < 400; i++)
        {
            availableSpots.Add(i);
        }
        
        for (var i = 0; i < 400; i++)
        {
            var spotChosen = coordinates.GetNumeral(400);
            var temp = availableSpots[i];
            availableSpots[i] = availableSpots[spotChosen];
            availableSpots[spotChosen] = temp;
        }

        var mineLocations = new List<int>();

        yield return null;

        //pick
        var tempMineCount = MineCount;
        for (var i = 0; i < tempMineCount; i++)
        {
            var num = availableSpots[i];
            
            if((num / 20)==0 || (num / 20)==19 || (num % 20)==0 || (num % 20) == 19)//no mines on the shore!
            {
                tempMineCount++;
                continue;
            }

            mineLocations.Add(num);
            _spawnMine((num / 20) - 10, (num % 20) - 10, data.IsCleared(num), completelyCleared);
        }

        yield return null;

        //reshuffle spots for entity placement
        for (var i = 0; i < 400; i++)
        {
            var spotChosen = coordinates.GetNumeral(400);
            var temp = availableSpots[i];
            availableSpots[i] = availableSpots[spotChosen];
            availableSpots[spotChosen] = temp;
        }

        //pick tree spots
        var treeCount = (int)(Mathf.Pow(CurrentDifficulty, 2f) / 1.1f) * (coordinates.GetNumeral() * 0.1f + 0.1f) + 20;
        var treeSpots = new List<int>();
        for(var i = 0; i < treeCount; i++)
        {
            treeSpots.Add(availableSpots[i]);
        }

        //reset spots
        availableSpots.Clear();
        for (var i = 0; i < 400; i++)
        {
            availableSpots.Add(i);
        }

        var wreckSpots = new List<int>();

        var vendorSpots = new List<int>();

        //plop down land & numbers
        for (var i = 0; i < 400; i++)
        {
            if ((i+1) % 50 == 0) //break every 50
            {
                yield return null;
            }
            if (mineLocations.Contains(i))
            {
                continue;
            }

            bool[] dirs; //0:right,1:up,2:left,3:down
            var adjacent = _mineCheck(i, mineLocations, out dirs);
            if (adjacent == -1)
            {
                Debug.LogError("Well, that's weird");
            }

            if (adjacent == 0)
            {
                _spawnGap((i / 20) - 10, (i % 20) - 10, completelyCleared || data.IsCleared(i));
                continue;
            }

            var tileType = 4;

            if (!dirs[0] && !dirs[1]) //right & up are gaps
            {
                tileType = 2;
            }
            else if (!dirs[1] && !dirs[2]) //up & left are gaps
            {
                tileType = 0;
            }
            else if (!dirs[2] && !dirs[3]) //left & down are gaps
            {
                tileType = 6;
            }
            else if (!dirs[3] && !dirs[0]) //down & right are gaps
            {
                tileType = 8;
            }
            else if (!dirs[0]) //right is gap
            {
                tileType = 5;
            }
            else if (!dirs[1]) //up is gap
            {
                tileType = 1;
            }
            else if (!dirs[2]) //left is gap
            {
                tileType = 3;
            }
            else if (!dirs[3]) //down is gap
            {
                tileType = 7;
            }

            if(tileType == wreckTileType)
            {
                wreckSpots.Add(i);
            }

            if(tileType == vendorTileType)
            {
                vendorSpots.Add(i);
            }

            _spawnLand((i / 20) - 10, (i % 20) - 10, tileType, adjacent - 1, data.IsCleared(i), completelyCleared);

            if (treeSpots.Contains(i))
            {
                _spawnTree((i / 20) - 10, (i % 20) - 10, completelyCleared);
            }
        }

        yield return null; //break

        //spawnWrecks
        if (hasWreck && wreckSpots.Count > 0)
        {
            //shuffle
            for(var i = 0; i < wreckSpots.Count; i++)
            {
                var spotChosen = coordinates.GetNumeral(wreckSpots.Count);
                var temp = wreckSpots[i];
                wreckSpots[i] = wreckSpots[spotChosen];
                wreckSpots[spotChosen] = temp;
            }
            
            var loc = wreckSpots[0];

            _spawnWreck((loc / 20) - 10, (loc % 20) - 10, completelyCleared);
        }

        //spawnVendors
        
        if (hasVendor && vendorSpots.Count > 0)
        {
            //shuffle
            for (var i = 0; i < vendorSpots.Count; i++)
            {
                var spotChosen = coordinates.GetNumeral(vendorSpots.Count);
                var temp = vendorSpots[i];
                vendorSpots[i] = vendorSpots[spotChosen];
                vendorSpots[spotChosen] = temp;
            }

            var loc = vendorSpots[0];

            _spawnVendor((loc / 20) - 10, (loc % 20) - 10, completelyCleared, rootZone ? 0 : (CurrentDifficulty - 1));
        }
    }

    public void Honk()
    {
        List<Tile> toHonk = new List<Tile>();
        for (var i = 0; i < 20; i ++)
        {
            for(var j = 0; j < 20; j++)
            {
                if((i!= 0 && i != 19 && j != 0 && j != 19) ||
                    tileMap[i,j] == null)
                {
                    continue;
                }

                toHonk.Add(tileMap[i, j]);
            }
        }

        StartCoroutine(_honkTiles(toHonk));
    }

    private IEnumerator _honkTiles(List<Tile> toHonk)
    {
        var numbers = Numberizer.GetDisplayNumbers();

        while (toHonk.Any())
        {
            var next = toHonk.First();
            toHonk.RemoveAt(0);

            if(next == null || next.gameObject == null)
            {
                continue;
            }

            next.Activate();

            yield return new WaitForSeconds(numbers.GetNumeral() * 0.04f);
        }
    }

    private void _spawnGap(int x, int y, bool cleared = false)
    {
        if (cleared)
        {
            tileMap[x + 10, y + 10] = null;
            return;
        }

        var spawned = Instantiate(GapPrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset, Quaternion.identity, LandRoot);

        spawned.SetParent(this);

        spawned.Shown = false;
        spawned.location = (x + 10) * 20 + y + 10;

        tiles.Add(spawned);
        tileMap[x + 10, y + 10] = spawned;
    }

    private void _spawnLand(int x, int y, int type, int number = -1, bool partlyCleared = false, bool cleared = false)
    {
        var spawned = Instantiate(LandPrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset, Quaternion.identity, LandRoot);

        spawned.SetParent(this);

        spawned.Shown = partlyCleared || cleared;
        spawned.GetComponent<SpriteRenderer>().sprite = LandSprites[type];
        ((Land)spawned).number = type;
        spawned.location = (x + 10) * 20 + y + 10;

        if (cleared || partlyCleared)
        {
            spawned.CoverRenderer.sprite = null;
        }

        if(!cleared && number >= 0)
        {
            var numberSpawn = Instantiate(NumberPrefab, new Vector3(x, y) + SpritesRoot.position + IslandCenterOffset, Quaternion.identity, SpritesRoot);

            spawned.SetParent(this);

            numberSpawn.GetComponent<SpriteRenderer>().sprite = NumberSprites[number];
            numberSpawn.SetActive(partlyCleared);

            ((Land)spawned).NumberRef = numberSpawn;
        }

        tiles.Add(spawned);
        tileMap[x + 10, y + 10] = spawned;
    }

    private void _spawnTree(int x, int y, bool cleared = false)
    {
        var spawned = Instantiate(TreePrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset + new Vector3(0, 0, -15f), Quaternion.identity, LandRoot);

        spawned.SetParent(this);

        if (cleared)
        {
            spawned.Show();
        }

        spawned.location = (x + 10) * 20 + y + 10;

        bits.Add(spawned);
    }

    private void _spawnWreck(int x, int y, bool cleared = false)
    {
        var spawned = Instantiate(WreckPrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset + new Vector3(0, 0, -15f), Quaternion.identity, LandRoot);

        spawned.SetParent(this);

        if (cleared)
        {
            spawned.Show();
        }

        spawned.location = (x + 10) * 20 + y + 10;

        bits.Add(spawned);
    }

    private void _spawnVendor(int x, int y, bool cleared = false, int level = 0)
    {
        var spawned = Instantiate(VendorPrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset + new Vector3(0, 0, -15f), Quaternion.identity, LandRoot);

        spawned.SetParent(this);
        spawned.Level = level;

        if (cleared)
        {
            spawned.Show();
        }

        spawned.location = (x + 10) * 20 + y + 10;

        bits.Add(spawned);
    }

    private void _spawnMine(int x, int y, bool partlyCleared = false, bool cleared = false)
    {
        var spawned = Instantiate(MinePrefab, new Vector3(x, y) + LandRoot.position + IslandCenterOffset, Quaternion.identity, LandRoot);

        spawned.SetParent(this);

        spawned.Shown = partlyCleared || cleared;
        spawned.GetComponent<SpriteRenderer>().sprite = cleared ? LandSprites[4] : MineSprite;
        spawned.location = (x + 10) * 20 + y + 10;
        ((Mine)spawned).ClearSprite = LandSprites[4];

        if (cleared || partlyCleared)
        {
            spawned.CoverRenderer.sprite = null;
        }

        tiles.Add(spawned);
        tileMap[x + 10, y + 10] = spawned;
    }

    private int _mineCheck(int location, List<int> mines, out bool[] dirChecks, bool doDirChecks = true)
    {
        //dir checks
        dirChecks = new bool[] {
            true, true, true, true
        };

        var locX = (location / 20);
        var locY = (location % 20);

        if (mines.Contains(location))
        {
            return -1;
        }

        var adjacent = 0;

        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                var checkPos = new Vector2(locX + i, locY + j);
                
                if (doDirChecks)
                {
                    //dir checks
                    bool[] substitute;
                    var miss = (checkPos.x < 0 || checkPos.x >= 20 || checkPos.y < 0 || checkPos.y >= 20) || _mineCheck((int)(checkPos.x * 20 + checkPos.y), mines, out substitute, false) == 0;
                    if (i == 1 && j == 0 && miss)
                    {
                        dirChecks[0] = false;
                    }
                    if (i == 0 && j == 1 && miss)
                    {
                        dirChecks[1] = false;
                    }
                    if (i == -1 && j == 0 && miss)
                    {
                        dirChecks[2] = false;
                    }
                    if (i == 0 && j == -1 && miss)
                    {
                        dirChecks[3] = false;
                    }
                }

                if (checkPos.x < 0 || checkPos.x >= 20 || checkPos.y < 0 || checkPos.y >= 20)
                {
                    continue;
                }

                if (mines.Contains((int)(checkPos.x * 20 + checkPos.y)))
                {
                    adjacent++;
                }
            }
        }

        return adjacent;
    }

    public void CheckIfIWon(bool forceWin = false)
    {
        var clearRating = 1f;
        var mineType = typeof(Mine);
        foreach(var tile in tiles)
        {
            if (tile == null || tile.gameObject == null)
                continue;

            var tileType = tile.GetType();
            if((tile.Flaged && tileType == mineType) ||
                (!tile.Flaged && tileType != mineType) || forceWin)
            {
                continue;
            }
            if (tile.Shown && tileType == mineType)
            {
                clearRating++;
                continue;
            }
            return;
        }

        //passed
        //trigger the rest of the land & gap tiles and clear them of numbers
        foreach (var tile in tiles)
        {
            if(tile != null && tile.gameObject != null)
                tile.Clear();
        }

        foreach(var bit in bits)
        {
            if(bit != null && bit.gameObject != null)
                bit.Show();
        }

        //grant extra points on perfect clear
        Director.PlayerScore += (10 * Mathf.Pow(CurrentDifficulty, 2)) / clearRating;

        zoneData.ClearAll();
    }

    public void UnfoldNeighborTilesOf(int location)
    {
        var workingList = neighborsOf(location);

        var startNew = !unfoldingList.Any();

        unfoldingList.AddRange(workingList.Where(item => !unfoldingList.Contains(item)));

        if (startNew)
        {
            StartCoroutine(UnfoldGap());
        }
    }

    private List<Tile> neighborsOf(int location)
    {
        var workingList = new List<Tile>();

        var x = location / 20;
        var y = location % 20;

        for (var i = -1; i < 2; i++)
        {
            if (x + i >= 20 || x + i < 0)
            {
                continue;
            }

            for (var j = -1; j < 2; j++)
            {
                if (y + j >= 20 || y + j < 0 ||
                    (i == 0 && j == 0))
                {
                    continue;
                }

                workingList.Add(tileMap[x + i, y + j]);
            }
        }

        return workingList;
    }

    public void TryNeighborClearOf(int location)
    {
        var neighbors = neighborsOf(location);

        var markedNeighbors = 0;
        var mineNeighbors = 0;

        var hiddenNeighbors = new List<Tile>();

        foreach (var neighbor in neighbors)
        {
            if (!neighbor)
            {
                continue;
            }

            if (neighbor.Flaged)
            {
                markedNeighbors++;
            }
            
            if(neighbor is Mine && !neighbor.Shown)
            {
                mineNeighbors++;
            }

            if (!neighbor.Flaged && !neighbor.Shown)
            {
                hiddenNeighbors.Add(neighbor);
            }
        }

        if (markedNeighbors == mineNeighbors)
        {
            foreach (var neighbor in neighbors)
            {
                if(neighbor != null && !neighbor.Shown)
                    neighbor.Activate();
            }
        }
    }

    private List<Tile> unfoldingList = new List<Tile>();

    private IEnumerator UnfoldGap()
    {
        yield return new WaitForSeconds(0.05f);

        while (unfoldingList.Any())
        {
            var next = unfoldingList.First();
            unfoldingList.RemoveAt(0);

            if (next != null && next.gameObject != null && !next.destroying)
            {
                next.Activate();
                if(unfoldingList.Any())
                    yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
