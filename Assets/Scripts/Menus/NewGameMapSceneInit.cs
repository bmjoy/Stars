﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/*
 *  Class used in NewGameMapScene
 */
public class NewGameMapSceneInit : MonoBehaviour
{
    private GameApp gameApp;
    private LevelLoader levelLoader;

    void Start()
    {
        Debug.Log("NewGameMapSceneInit");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();

        // find and put files to dropdown list
        Dropdown mapToLoadDropdown = GameObject.Find("MenuCanvas/DropdownCanvas/MapToLoadDropdown").GetComponent<Dropdown>();
        string[] foundFiles = Directory.GetFiles(gameApp.startMapsPath, "*.json");
        foreach (string foundFile in foundFiles)
        {
            mapToLoadDropdown.options.Add(new Dropdown.OptionData() { text = Path.GetFileNameWithoutExtension(foundFile) });
        }
    }

    public void Back()
    {
        levelLoader.Back("MainMenuScene");
    }

    public void Next()
    {
        gameApp.PersistAllParameters("NewGameMapScene");
        levelLoader.LoadLevel("NewGameScene");
    }
}
