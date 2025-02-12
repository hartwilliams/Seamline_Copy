using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveUnit
{
    public float[] playerPosition;
    public int playerHealth;
    public float playerYarn;
    public int potions;
    public string currentSceneName;
    public bool firstRiftDone; 
    public bool[] levelsReached;
    public bool[] doorOpened;

    public SaveUnit()
    {
        // Load data to be saved
        playerPosition = new float[3] {
            PlayerStats._instance.transform.position.x,
            PlayerStats._instance.transform.position.y,
            PlayerStats._instance.transform.position.z
        };
        playerHealth = PlayerStats._instance.currentHealth;
        playerYarn = PlayerStats._instance.currentYarnCount;
        potions = PlayerStats._instance.potions;
        currentSceneName = SceneManager.GetActiveScene().name;
        levelsReached = PlayerStats._instance.levelsReached;
        //wallOpenClose[] doorControllers = SaveSystem._instance.doorControllers;
        //doorOpened = new bool[doorControllers.Length];
        //for (int i = 0; i < doorControllers.Length; i++)
        //{
        //    doorOpened[i] = doorControllers[i].isOpened();
        //    Debug.Log(doorControllers[i].isOpened());
        //}

        firstRiftDone = PlayerStats._instance.firstRiftDone; 
    }
}
