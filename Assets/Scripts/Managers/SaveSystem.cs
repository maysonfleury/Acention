using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    public static void SaveGameState (RigidBodyMovement player, UIManager ui)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.info";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(player, ui);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveSettings (SettingsManager settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.info";
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsManager data = settings;

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGameState ()
    {
        string path = Application.persistentDataPath + "/player.info";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogWarning($"[SaveSystem]: Game save file not found in {path}");
            return null;
        }
    }

    public static SettingsManager LoadSettings ()
    {
        string path = Application.persistentDataPath + "/settings.info";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsManager settings = formatter.Deserialize(stream) as SettingsManager;
            stream.Close();

            return settings;
        }
        else
        {
            Debug.LogWarning($"[SaveSystem]: Settings save file not found in {path}");
            return null;
        }
    }

    public static void DeleteGameState ()
    {
        string path = Application.persistentDataPath + "/player.info";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogWarning("[SaveSystem]: No Game save file found to delete.");
        }
    }
}
