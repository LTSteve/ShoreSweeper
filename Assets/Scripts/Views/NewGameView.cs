using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameView : SubmenuView
{
    public override void ClearView()
    {
        //do something to clear the view
    }

    public void TryStartGame()
    {
        //get the name
        var name = CleanFileName(transform.Find("Layout/NameRow/InputField/Text").GetComponent<Text>().text).Trim();

        //TODO: the rest of the settings
        
        if (string.IsNullOrEmpty(name))
        {
            //TODO: feedback
            return;
        }

        var newSave = new SaveFile();

        newSave.score = 0;
        newSave.shipData = ShipData.StarterPackage;

        newSave.playerName = name;

        Director.D.StartNewGame(newSave);
    }

    private string CleanFileName(string name)
    {
        string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
    }
}
