using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameView : SubmenuView
{

    public SaveButton SaveRowPrefab;
    public string[] ShipNames;

    private int rowCount = 0;
    private List<string> files = new List<string>();

    private int selected = -1;

    protected new void Start()
    {
        base.Start();
        ClearView();
    }

    private IEnumerator _pullFileData(string[] files)
    {
        foreach(var file in files)
        {
            var save = SaveSystem.LoadFile(file);
            _addSaveRow(save);
            yield return null;
        }
    }

    private void _addSaveRow(SaveFile save)
    {
        var root = Instantiate(SaveRowPrefab, transform.Find("Scroll View/Viewport/Content"));
        //root.transform.localPosition = new Vector3(root.transform.localPosition.x, root.transform.localPosition.y - 170 * rowCount, root.transform.localPosition.z);
        root.Index = rowCount;
        files.Add(save.playerName);
        rowCount++;

        root.transform.Find("Padding/Name/Value").GetComponent<Text>().text = save.playerName;
        root.transform.Find("Padding/Money/Value").GetComponent<Text>().text = "$" + (int)save.score;
        root.transform.Find("Padding/Time/Value").GetComponent<Text>().text = (int)((save.time / 60) / 60) + "h " + (int)((save.time / 60) % 60) + "m";
        root.transform.Find("Padding/Ship/Value").GetComponent<Text>().text = ShipNames[Mathf.Clamp(save.shipData.hull,0,ShipNames.Length)];
    }

    public override void ClearView()
    {
        foreach(Transform child in transform.Find("Scroll View/Viewport/Content"))
        {
            Destroy(child.gameObject);
        }
        rowCount = 0;
        files.Clear();
        selected = -1;

        var folderPath = Application.persistentDataPath;
        if (Directory.Exists(folderPath))
        {
            var files = Directory.GetFiles(folderPath);

            StartCoroutine(_pullFileData(files));
        }
    }

    public void SelectRow(SaveButton row)
    {
        var texts = row.GetComponentsInChildren<Text>();
        foreach(var text in texts)
        {
            text.color = Color.white;
        }

        transform.Find("Start").GetComponent<Button>().interactable = true;

        selected = row.Index;
    }

    public void LoadFile()
    {
        if(selected == -1 || files.Count == 0)
        {
            return;
        }

        Director.D.LoadGame(files[selected]);
    }
}
