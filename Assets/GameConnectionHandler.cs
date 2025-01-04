
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
using Unity.Netcode.Transports.UTP;
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

    private string selectedGameAddress;
    private Dictionary<string, string> availableGames = new Dictionary<string, string>(); // gameName -> IP:Port
    private void Start()
    {
        ConnectButton.onClick.AddListener(ConnectToServer);
        CreateButton.onClick.AddListener(CreateNewGame);

        ConnectButton.interactable = false;
        CreateButton.interactable = false;
    }

    private void Update()
    {
        bool hasName = !string.IsNullOrEmpty(InputName.text);
        ConnectButton.interactable = hasName && !string.IsNullOrEmpty(selectedGameAddress);
        CreateButton.interactable = hasName;
    }

    private void ConnectToServer()
    {
        if (string.IsNullOrEmpty(selectedGameAddress))
        {
            Debug.LogError("No game selected to connect!");
            return;
        }

        // Split address into IP and Port
        string[] addressParts = selectedGameAddress.Split(':');
        if (addressParts.Length != 2)
        {
            Debug.LogError("Invalid game address format!");
            return;
        }

        string ip = addressParts[0];
        int port = int.Parse(addressParts[1]);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)port);
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log($"Connecting to server at {selectedGameAddress}...");
        }
        else
        {
            Debug.LogError("Failed to connect to server.");
        }
    }

    private void CreateNewGame()
    {
        if (string.IsNullOrEmpty(InputName.text)) return;

        string playerName = InputName.text;
        if (!NetworkManager.Singleton.StartHost())
        {
            Debug.LogError("Failed to start host.");
            return;
        }
        //NetworkManager.Singleton.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = InputName.gameObject });
        NetworkManager.Singleton.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);

    }

    public void AddGameToList(string gameName, string address)
    {
        Debug.Log($"Adding game: {gameName} at {address}");

        if (!availableGames.ContainsKey(gameName))
        {
            availableGames[gameName] = address;

            // Instantiate the prefab
            GameObject newGameItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
            Debug.Log($"New game item instantiated: {newGameItem.name}");

            TMP_Text gameText = newGameItem.GetComponentInChildren<TMP_Text>();
            if (gameText != null)
            {
                gameText.text = gameName;
                Debug.Log($"Game name set to: {gameName}");
            }
            else
            {
                Debug.LogError("GameListItemPrefab is missing a TMP_Text component!");
            }

            Button button = newGameItem.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectGame(gameName, address));
                Debug.Log("Button listener added.");
            }
            else
            {
                Debug.LogError("GameListItemPrefab is missing a Button component!");
            }

            // Force update layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(AvailableGamesScrollView.content.GetComponent<RectTransform>());
        }
        else
        {
            Debug.Log($"Game {gameName} already exists in the list.");
        }
    }



    private void SelectGame(string gameName, string address)
    {
        selectedGameAddress = address;
        Debug.Log($"Selected game: {gameName} at {address}");
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

