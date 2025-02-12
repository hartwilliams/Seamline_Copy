using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem _instance;
    //static private string[] doorNames;
    //[HideInInspector] public wallOpenClose[] doorControllers;
    //if we want to create multiple save files, create a list of objects and set which one to save to/load from
    public static string currentFileName = "save0"; //initial file name

    private static string[] initialFileNames = { "", "", "" };
    public static List<string> listSavedFiles = new List<string>(initialFileNames);

    private void Awake()
    {
        _instance = this;

        //doorNames = new string[] { "Tilemap Final Boss Walls" };
        //if (String.Compare(SceneManager.GetActiveScene().name, "sanctuary") != 0)
        //{
        //    Debug.Log("level not sanctuary");
        //    doorControllers = new wallOpenClose[doorNames.Length];
        //    for (int i = 0; i < doorNames.Length; i++)
        //    {
        //        doorControllers[i] = GameObject.Find(doorNames[i]).GetComponent<wallOpenClose>();
        //    }
        //}
    }

    public static void CreateSave()
    {
        Time.timeScale = 0f;

        BinaryFormatter formatter = new BinaryFormatter();

        //string savePath = Application.persistentDataPath + "/" + DateTime.Now.ToString("hh-mm-ss") + ".sav";
        string savePath = Application.persistentDataPath + "/" + currentFileName + ".sav";
        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, new SaveUnit());
        stream.Close();

        Debug.Log(savePath);

        Time.timeScale = 1f;

        listSavedFiles[int.Parse(currentFileName[4].ToString())] = currentFileName;
    }

    private static SaveUnit GetSavedData(string savePath)
    {
        SaveUnit data = null;
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(savePath, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveUnit;
            stream.Close();
        }
        else
        {
            Debug.Log("Save Not Found");
        }
        return data;
    }

    public static void LoadSave()
    {
        Time.timeScale = 0f;

        string savePath = Application.persistentDataPath + "/" + currentFileName + ".sav";

        if (File.Exists(savePath))
        {
            SaveUnit data = GetSavedData(savePath);

            Vector3 playerPos = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            // if reloading save to a reached level, set position at the player position
            PlayerStats._instance.transform.position = playerPos;

            PlayerStats._instance.currentHealth = data.playerHealth;
            PlayerStats._instance.currentYarnCount = data.playerYarn;
            PlayerStats._instance.potions = data.potions;
            PlayerStats._instance.levelsReached = data.levelsReached;
            PlayerStats._instance.firstRiftDone = data.firstRiftDone;
            Debug.Log(SceneManager.GetActiveScene().name);
            // if(String.Compare(SceneManager.GetActiveScene().name, "sanctuary") != 0)
            // {


            //Debug.Log(data.doorOpened.Length + " Door Data Retrieved");
            //_instance.doorControllers = new wallOpenClose[doorNames.Length];
            //for (int i = 0; i < doorNames.Length; i++)
            //{
            //    _instance.doorControllers[i] = GameObject.Find(doorNames[i]).GetComponent<wallOpenClose>();
            //}
            //for (int i = 0; i < data.doorOpened.Length; i++)
            //{
            //    _instance.doorControllers[i].SetDoorsOnLoad(data.doorOpened[i]);
            //}
            // }
        }
        else
        {
            Debug.Log("Save Not Found");
        }

        Time.timeScale = 1f;
    }

    public static void SetPlayerPosition()
    {
        string savePath = Application.persistentDataPath + "/" + currentFileName + ".sav";
        SaveUnit data = GetSavedData(savePath);

        if (data != null)
        {
            PlayerStats._instance.transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        }

    }

    public static string LoadSavedSceneName()
    {
        string savePath = Application.persistentDataPath + "/" + currentFileName + ".sav";
        SaveUnit data = GetSavedData(savePath);
        //if data not null
        if (data != null)
        {
            return data.currentSceneName;
        }
        else
        {
            return "Tutorial Level";
        }
    }

    public static void GetInitialPositions()
    {
        if (String.Compare(SceneManager.GetActiveScene().name, "Tutorial Level") == 0)
        {
            PlayerStats._instance.transform.position = new Vector3(18.8f, 23.1f, -1f);
        }
        else if (String.Compare(SceneManager.GetActiveScene().name, "sanctuary") == 0)
        {
            PlayerStats._instance.transform.position = new Vector3(-75.3f, 46.7f, 0f);
        }
        else if (String.Compare(SceneManager.GetActiveScene().name, "Second Level") == 0)
        {
            PlayerStats._instance.transform.position = new Vector3(32.6f, 27.3f, -1f);
        }
    }

    public void SetFileIndex(int index)
    {
        if (string.Equals(listSavedFiles[index], ""))
        {
            currentFileName = "save" + index;
        }
        else
        {
            currentFileName = listSavedFiles[index];
        }
    }

    public void DeleteSave(int index)
    {
        if (!string.Equals(listSavedFiles[index], ""))
        {
            listSavedFiles[index] = "";
        }
    }

    public static int GetNumberSavedFiles()
    {
        return listSavedFiles.FindAll(file => !string.Equals(file, "")).Count;
    }
}
