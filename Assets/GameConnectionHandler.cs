
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
    private HashSet<string> discoveredGames = new HashSet<string>();
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
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton не настроен или отсутствует.");
            return;
        }

        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Не удалось запустить сервер.");
            return;
        }

        if (NetworkManager.Singleton.SceneManager == null)
        {
            Debug.LogError("SceneManager не настроен.");
            return;
        }
        //NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = InputName.gameObject });
        NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);

    }

    public void AddGameToList(string gameInfo)
    {
        if (!discoveredGames.Contains(gameInfo))
        {
            discoveredGames.Add(gameInfo);

            GameObject newGameItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
            Text gameText = newGameItem.GetComponentInChildren<Text>();
            gameText.text = gameInfo;
        }
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("NetworkManager завершил работу.");
        }
    }
}

