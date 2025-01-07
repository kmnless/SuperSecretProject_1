
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameConnectionHandler : MonoBehaviour
{
    private const string SceneName = "GameSetup";
    private const ushort GamePort = 2282; // also kostil ??

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

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, GamePort);

        string nickname = InputName.text.Trim();
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogError("Nickname cannot be empty!");
            return;
        }

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(nickname);
        Debug.Log($"ConnectionData set with nickname: {nickname}");

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log($"Connecting to server at {selectedGameAddress}...");

        }
        else
        {
            Debug.LogError("Failed to connect to server.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected, waiting for server to manage the scene...");
    }

    private void CreateNewGame()
    {
        if (string.IsNullOrEmpty(InputName.text)) return;

        string playerName = InputName.text;
        GlobalVariableHandler.Instance.ServerName = playerName;

        SceneManager.LoadScene(SceneName);
    }

    public void AddGameToList(string gameName, int playerCount, int maxPlayers, string address)
    {
        if (!availableGames.ContainsKey(gameName))
        {
            availableGames[gameName] = address;
            GameObject newGameItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);

            RectTransform rectTransform = newGameItem.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(AvailableGamesScrollView.viewport.rect.width, 50);

            TMP_Text gameText = newGameItem.GetComponentInChildren<TMP_Text>();
            gameText.text = $"{gameName}\t{playerCount}/{maxPlayers}";
            Button button = newGameItem.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => SelectGame(gameName, address));
            LayoutRebuilder.ForceRebuildLayoutImmediate(AvailableGamesScrollView.content.GetComponent<RectTransform>());
        }
    }

    private void SelectGame(string gameName, string address)
    {
        selectedGameAddress = address;
    }


    private void OnDestroy()
    {
        
    }
}

