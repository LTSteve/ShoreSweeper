using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapView : MonoBehaviour
{
    public static Dictionary<ProceduralGenerator, Image> Monitored = new Dictionary<ProceduralGenerator, Image>();

    public static void AddMonitored(ProceduralGenerator toMonitor)
    {
        if (toMonitor == null || Monitored.ContainsKey(toMonitor)) return;

        Monitored.Add(toMonitor, null);
    }

    public Image[] DotPrefabs;
    public Transform Panel;

    public bool doOpen;
    private Vector3 startingPos;
    private Vector3 zero = Vector3.zero;
    private bool opening;
    private bool open;

    private PlayerController player;

    public void Start()
    {
        startingPos = transform.parent.localPosition;

        ToggleOpen();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || doOpen)
        {
            ToggleOpen();
        }

        if (player == null)
        {
            player = PlayerController.Instance;
        }

        var MonitoredKeys = new List<ProceduralGenerator>(Monitored.Keys);

        foreach (var key in MonitoredKeys)
        {
            if (key == null || Monitored[key] != null) continue;

            var zonestate = key.zoneData.cleared ? 2 :
                (key.Honked ? 1 : 0);

            Monitored[key] = Instantiate(DotPrefabs[zonestate], Panel);
        }

        if (!Camera.main)
        {
            return;
        }

        var range = Camera.main.orthographicSize;

        var center = player.transform.position;

        foreach(var zone in MonitoredKeys)
        {
            var dot = Monitored[zone];

            if(zone == null || zone.gameObject == null)
            {
                Monitored.Remove(zone);
                Destroy(dot.gameObject);
                continue;
            }

            var zonestate = zone.zoneData.cleared ? 2 :
                (zone.Honked ? 1 : 0);

            if(dot.sprite != DotPrefabs[zonestate].sprite)
            {
                Destroy(dot.gameObject);
                Monitored[zone] = dot = Instantiate(DotPrefabs[zonestate], Panel);
            }

            dot.transform.localPosition = (zone.transform.position + zone.IslandCenterOffset - center) * (20f/(Mathf.Pow(range, 1.5f)));

        }
    }

    public void ToggleOpen()
    {
        if (!opening)
        {
            doOpen = false;
            open = !open;

            opening = true;

            StartCoroutine(_shiftPosition(open));
        }
    }

    private IEnumerator _shiftPosition(bool open)
    {
        var timer = 0.2f;

        while (timer > 0)
        {

            if (open)
            {
                transform.parent.localPosition = Vector3.Lerp(startingPos, zero, 1f - (timer / 0.2f));
            }
            else
            {
                transform.parent.localPosition = Vector3.Lerp(zero, startingPos, 1f - (timer / 0.2f));
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        if (open)
        {
            transform.parent.localPosition = zero;
        }
        else
        {
            transform.parent.localPosition = startingPos;
        }

        opening = false;
    }
}
