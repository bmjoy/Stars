﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
 *  Class used in LoadGameScene
 */
public class LoadGameSceneInit : MonoBehaviour
{
    private GameApp gameApp;
    private LevelLoader levelLoader;
    private RectTransform dynamicGrid;
    private List<GameObject> playersToAddToGame;

    void Start()
    {
        Debug.Log("LoadGameSceneInit");

        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        dynamicGrid = GameObject.Find("ListCanvas/ListPanel/ListGrid").GetComponent<RectTransform>();

        playersToAddToGame = new List<GameObject>();

        // gat and parse saved json file
        string path = gameApp.savedGamesPath + gameApp.GetInputField("GameToLoad");
        JObject gameParsed = null;
        try
        {
            gameParsed = gameApp.ReadJsonFile(path);
        }
        catch(Exception e)
        {
            Debug.Log("LoadGameSceneInit error: " + e.Message);
            Debug.Log(e.StackTrace);
            Back();
        }

        if (gameParsed == null)
        {
            Debug.Log("LoadGameSceneInit gameParsed is null");
            Back();
        }

        JArray playersJson;
        int maxPlayers;
        try
        {
            playersJson = (JArray)gameParsed["players"];
            maxPlayers = (int)gameParsed["info"]["maxPlayers"];
        }
        catch (Exception e)
        {
            Debug.Log("LoadGameSceneInit error: " + e.Message);
            Back();
            return;
        }

        if (maxPlayers != playersJson.Count)
        {
            Debug.Log("LoadGameSceneInit error: maxPlayers != playersJson.Count");
            Back();
            return;
        }

        // players' names and races shouldn't change between sessions
        foreach (JObject playerJson in playersJson)
        {
            GameObject newPlayer = Instantiate(gameApp.PlayerMenuPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            newPlayer.transform.SetParent(dynamicGrid.transform, false);
            EventTrigger trigger = newPlayer.GetComponentInChildren<Toggle>().gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((eventData) => {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(GameObject.Find("EventSystem"));
            });
            trigger.triggers.Add(entry);
            newPlayer.transform.Find("PlayerNameInput").GetComponent<InputField>().text = (string)playerJson["name"];
            newPlayer.transform.Find("PlayerNameInput").GetComponent<InputField>().enabled = false;

            //newPlayer.transform.Find("PlayerRaceInput").GetComponent<InputField>().text = (string)playerJson["playerMain"]["race"];
            //newPlayer.transform.Find("PlayerRaceInput").GetComponent<InputField>().enabled = false;
            playersToAddToGame.Add(newPlayer);
        }
    }

    public void Back()
    {
        levelLoader.Back("LoadGameMapScene");
    }

    public void Create()
    {
        List<GameApp.PlayerMenu> playerMenuList = new List<GameApp.PlayerMenu>();
        foreach (GameObject player in playersToAddToGame)
        {
            string tempType;
            if (player.transform.Find("PlayerTypeInput").GetComponent<Toggle>().isOn)
            {
                tempType = "L";
            }
            else tempType = "R";
            playerMenuList.Add(new GameApp.PlayerMenu
            {
                name = player.transform.Find("PlayerNameInput").GetComponent<InputField>().text,
                password = player.transform.Find("PlayerPassInput").GetComponent<InputField>().text,
                //race = player.transform.Find("PlayerRaceInput").GetComponent<InputField>().text,
                playerType = tempType
            });
        }

        Debug.Log("LoadGameSceneInit create, players: " + playerMenuList.Count);
        gameApp.SavePlayersFromMenu(playerMenuList);
        ServerNetworkManager serverNetworkManager = GameObject.Find("ServerNetworkManager").GetComponent<ServerNetworkManager>();
        serverNetworkManager.SetupLoadGame();
    }
}
