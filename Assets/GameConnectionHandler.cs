
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameConnectionHandler : MonoBehaviour
{
    private const string SceneName = "GameSetup";

    [SerializeField] private TMP_InputField InputName;
    [SerializeField] private Button ConnectButton;
    [SerializeField] private Button CreateButton;
    [SerializeField] private ScrollRect AvailableGamesScrollView;
    [SerializeField] private GameObject GameListItemPrefab;

    private void Start()
    {
        ConnectButton.onClick.AddListener(ConnectToServer);
        CreateButton.onClick.AddListener(CreateNewGame);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(InputName.text))
        {
            ConnectButton.interactable = false;
            CreateButton.interactable = false;
        }
        else
        {
            ConnectButton.interactable = true;
            CreateButton.interactable = true;
        }
    }

    private void ConnectToServer()
    {
        if (string.IsNullOrEmpty(InputName.text)) return;

        string playerName = InputName.text;

        NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = InputName.gameObject });

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Connecting...");
        }
        else
        {
            Debug.Log("Can't connect");
        }
    }

    private void CreateNewGame()
    {
        if (string.IsNullOrEmpty(InputName.text)) return;

        string playerName = InputName.text;

        NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = InputName.gameObject });
        NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);

    }

    public void AddGameToList(string gameName)
    {
        GameObject newGameItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
        Text gameText = newGameItem.GetComponentInChildren<Text>();
        gameText.text = gameName;
    }
}

