using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static void CreateSave()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        //string savePath = Application.persistentDataPath + "/" + DateTime.Now.ToString("hh-mm-ss") + ".sav";
        string savePath = Application.persistentDataPath + "/save_default.sav";
        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, new SaveUnit());
        stream.Close();

        Debug.Log(savePath);
    }

    public static void LoadSave()
    {
        string savePath = Application.persistentDataPath + "/save_default.sav";

        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(savePath, FileMode.Open);

            SaveUnit data = formatter.Deserialize(stream) as SaveUnit;
            stream.Close();

            Vector3 playerPos = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            Debug.Log(playerPos);
        }
        else
        {
            Debug.Log("Save Not Found");
        }
    }
}
