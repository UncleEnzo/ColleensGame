using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager
{
    public static bool Save(string saveName, object saveData)
    {
        Debug.Log("Saving game: " + saveName);
        BinaryFormatter formatter = GetBinaryFormatter();
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

        FileStream file = File.Create(path);
        formatter.Serialize(file, saveData);
        file.Close();
        return true;
    }

    public static object Load(string path)
    {
        Debug.Log("Loading game: " + path);
        if (!File.Exists(path))
        {
            return null;
        }
        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }
    }

    public static void Delete(string path)
    {
        Debug.Log("Deleting game: " + path);
        if (!File.Exists(path))
        {
            Debug.LogError("Could not find game to delete. Doing nothing");
        }
        File.Delete(path);
        BinaryFormatter formatter = GetBinaryFormatter();
        Debug.Log("Verifying delete");
        try
        {
            FileStream file = File.Open(path, FileMode.Open);
            object save = formatter.Deserialize(file);
            file.Close();
            Debug.Log("Delete failed");
        }
        catch
        {
            Debug.Log("Delete was a success");
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }

}
