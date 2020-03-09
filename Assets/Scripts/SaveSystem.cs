using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveZones(SaveFile data)
    {

        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/" + data.playerName + ".sv";
        var stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveFile LoadZones(string playerName = "zones")
    {
        var path = Application.persistentDataPath + "/" + playerName + ".sv";
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);
            var data = (SaveFile)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }

        return null;
    }
}