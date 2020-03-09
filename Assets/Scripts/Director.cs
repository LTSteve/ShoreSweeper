using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour {

    public static ProceduralGenerator[,] ActiveZones = new ProceduralGenerator[5,5];

    public static Numberizer Coordinates;

    public static Director D;

    public static float PlayerScore = 1000000;

    public static int[] RegenerateBounds =
    {
        -120,120,//x
        -120,120//y
    };

    public static Dictionary<int, ZoneData> worldData = new Dictionary<int, ZoneData>();

    public TextAsset GameDataText;
    public GameData GameData;
    public ProceduralGenerator ZonePrefab;
    public string seed = "asdf";
    public Texture2D[] CursorTextures;

    public Transform Player;

    public Vector2 Center = Vector2.zero;

    public bool Loaded = false;

    private Camera myCamera;

    public LoadingView LoadingScreen;

    private SaveFile saveFile;


    public void Start()
    {
        D = this;

        DontDestroyOnLoad(this.gameObject);

        myCamera = GetComponent<Camera>();

        CursorBro.Do(0);
    }

    public void LoadWorld()
    {
        StartCoroutine(_loadAsync());
    }

    private IEnumerator _loadAsync(SaveFile newFile = null, string name = null)
    {
        yield return null; // wait for everything to load
        LoadingView.Enable();

        LoadingView.Set(.25f, "Loading Game Data...");
        GameData = JsonUtility.FromJson<GameData>(GameDataText.text);

        yield return null;
        LoadingView.Set(0.3f, "Initializing Randomization...");

        Coordinates = new Numberizer(Numberizer.Numberize(seed), true);

        yield return null;
        LoadingView.Set(0.35f, "Loading Savegame...");

        saveFile = SaveSystem.LoadZones(name);

        yield return null;

        if (saveFile != null)
        {
            LoadZonesFromSave(saveFile);

            yield return null;
        }
        else
        {
            saveFile = newFile;
        }

        LoadingView.Set(0.4f, "Generating Islands[0/25]...");

        yield return StartCoroutine(_genNewZones());

        LoadingView.Set(0.99f, "Spawning Player...");

        _spawnPlayer(ActiveZones[2, 2], saveFile);

        yield return null;
        LoadingView.Set(1f);

        Loaded = true;
        yield return null;
    }

    public void Update()
    {
        if (!Loaded)
        {
            return;
        }

        if (myCamera.enabled)
        {
            myCamera.enabled = false;
        }

        var pX = Player.transform.position.x;
        var pY = Player.transform.position.y;

        var xmov = 0;
        var ymov = 0;

        if(pX > RegenerateBounds[1])
        {
            xmov = 1;
        }
        if(pX < RegenerateBounds[0])
        {
            xmov = -1;
        }

        if (pY > RegenerateBounds[3])
        {
            ymov = 1;
        }
        if (pY < RegenerateBounds[2])
        {
            ymov = -1;
        }

        if(xmov != 0 || ymov != 0)
        {
            StartCoroutine(_shiftZones(xmov,ymov));
        }

        Save();
    }

    public static ProceduralGenerator CenterZone()
    {
        var playerLocation = PlayerController.Instance.transform.position;

        var dCenter = new Vector3(D.Center.x, D.Center.y);

        var locationDiff = playerLocation - dCenter;

        var halfBlock = 60;

        return ActiveZones[2 + ((int)locationDiff.x/halfBlock), 2 + ((int)locationDiff.y/halfBlock)];
    }

    private IEnumerator _shiftZones(int xmov, int ymov)
    {
        Loaded = false;

        if (xmov != 0 || ymov != 0)
        {
            if(xmov != 0)
            {
                //destroy what i'm leaving
                for(var j = 0; j < 5; j++)
                {
                    Destroy(ActiveZones[xmov > 0 ? 0 : 4, j].gameObject);
                    yield return null;
                }

                //move everything over
                for(var i = xmov > 0 ? 0 : 4; (xmov > 0 && i < 4) || (xmov < 0 && i > 0); i += xmov)
                {
                    for(var j = 0; j < 5; j++)
                    {
                        ActiveZones[i, j] = ActiveZones[i + xmov, j];
                    }
                }

                //free up new space
                for (var j = 0; j < 5; j++)
                {
                    ActiveZones[xmov > 0 ? 4 : 0, j] = null;
                }
            }
            if (ymov != 0)
            {
                //destroy what i'm leaving
                for (var i = 0; i < 5; i++)
                {
                    Destroy(ActiveZones[i, ymov > 0 ? 0 : 4].gameObject);
                    yield return null;
                }

                //move everything over
                for (var j = ymov > 0 ? 0 : 4; (ymov > 0 && j < 4) || (ymov < 0 && j > 0); j += ymov)
                {
                    for (var i = 0; i < 5; i++)
                    {
                        ActiveZones[i, j] = ActiveZones[i, j + ymov];
                    }
                }

                //free up new space
                for (var i = 0; i < 5; i++)
                {
                    ActiveZones[i, ymov > 0 ? 4 : 0] = null;
                }
            }
        }

        Coordinates = Coordinates.GetNeighbor(xmov, ymov);
        RegenerateBounds[0] = RegenerateBounds[0] + 120 * xmov;
        RegenerateBounds[1] = RegenerateBounds[1] + 120 * xmov;
        RegenerateBounds[2] = RegenerateBounds[2] + 120 * ymov;
        RegenerateBounds[3] = RegenerateBounds[3] + 120 * ymov;
        Center += new Vector2(xmov*120,ymov*120);

        yield return StartCoroutine(_genNewZones());

        Loaded = true;
    }

    private IEnumerator _genNewZones()
    {
        for (var i = 0; i < ActiveZones.GetLength(0); i++)
        {
            for (var j = 0; j < ActiveZones.GetLength(1); j++)
            {
                if (ActiveZones[i,j] != null)
                {
                    continue;
                }

                var num = (i * 5 + j + 1);
                LoadingView.Set(0.4f + num * .022f, "Generating Islands[" + num + "/25]...");

                var newZoneCoords = Coordinates.GetNeighbor(i, j);

                var zoneData = worldData.ContainsKey(newZoneCoords.seed) ? worldData[newZoneCoords.seed] : null;
                if(zoneData == null)
                {
                    zoneData = new ZoneData(newZoneCoords.seed);
                    worldData[newZoneCoords.seed] = zoneData;
                }

                ActiveZones[i, j] = Instantiate(ZonePrefab, new Vector3((i - 2) * 120 + Center.x, (j - 2) * 120 + Center.y), Quaternion.identity, transform);

                StartCoroutine(ActiveZones[i, j].Activate(Coordinates.GetNeighbor(i, j), zoneData));

                MinimapView.AddMonitored(ActiveZones[i, j]);
                yield return null;
            }
        }
    }

    private void _spawnPlayer(ProceduralGenerator zone, SaveFile saveFile)
    {
        var player = PlayerController.Instance;

        Player = player.transform;

        Player.position = zone.transform.position + zone.IslandCenterOffset + new Vector3(0, -15f, 0);
        Player.gameObject.SetActive(true);

        player.SetHull(saveFile.shipData.hull, false);
        player.SetSails(saveFile.shipData.sails);
    }


    private static Coroutine queuedSave;

    public static void Save()
    {
        if (!D.Loaded)
        {
            if(queuedSave == null)
            {
                queuedSave = D.StartCoroutine(_saveLater());
            }
            return;
        }
        var zones = new List<ZoneData>();

        foreach(var value in worldData.Values)
        {
            zones.Add(value);
        }

        var save = D.saveFile == null ? new SaveFile() : D.saveFile;
        var player = D.Player.GetComponent<PlayerController>();

        save.zoneData = zones.ToArray();
        save.score = PlayerScore;
        save.playercoords = ActiveZones[0, 0].zoneData.coordinates;
        save.shipData = new ShipData
        {
            hull = player.GetHullType(),
            sails = player.GetSails()
        };
        save.time = save.time + Time.time;
        save.lastPlayed = DateTime.Now.Ticks;

        SaveSystem.SaveZones(save);
    }

    private static IEnumerator _saveLater()
    {
        while (!D.Loaded)
        {
            yield return null;
        }

        queuedSave = null;
        Save();
    }
    
    private void LoadZonesFromSave(SaveFile save)
    {
        var zones = save.zoneData;

        foreach (var zonedata in zones)
        {
            worldData.Add(zonedata.coordinates, zonedata);
        }

        PlayerScore = save.score;
        Coordinates = new Numberizer(save.playercoords, true);
    }

    public void StartNewGame(SaveFile newFile)
    {
        Loaded = false;
        
        StartCoroutine(_startGame(0, newFile));
    }

    public void LoadGame(string name)
    {
        Loaded = false;

        StartCoroutine(_startGame(0, null, name));
    }

    private IEnumerator _startGame(int sceneIndex, SaveFile file = null, string loadingName = null)
    {
        //open loading screen
        LoadingView.Enable();
        LoadingView.Set(0f, "Loading Game Scene...");

        //async load level
        var operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            LoadingView.Set((operation.progress / 0.9f)*0.25f);

            yield return null;
        }

        //generate level & close loading screen

        yield return StartCoroutine(_loadAsync(file, loadingName));
    }
}
