
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


        //AddGameToList("Test Game 1", "127.0.0.1:7777");
        //AddGameToList("Test Game 2", "127.0.0.1:8888");
        //AddGameToList("Test Game 3", "127.0.0.1:9999");
        //Debug.Log("Testing Instantiate...");
        //GameObject testItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
        //GameObject testItem1 = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
        //GameObject testItem2 = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
        //if (testItem != null)
        //{
        //    Debug.Log($"Test item instantiated: {testItem.name}");
        //    RectTransform rectTransform = testItem.GetComponent<RectTransform>();
        //    if (rectTransform != null)
        //    {
        //        rectTransform.sizeDelta = new Vector2(AvailableGamesScrollView.viewport.rect.width, 50); // Ширина = ширине Viewport, высота = 50
        //    }



        //    TMP_Text text = testItem.GetComponentInChildren<TMP_Text>();
        //    if (text != null)
        //    {
        //        text.text = "Test Game";
        //    }

        //    // Принудительное обновление макета
        //    RectTransform contentRect = AvailableGamesScrollView.content.GetComponent<RectTransform>();
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        //}
        //else
        //{
        //    Debug.LogError("Failed to instantiate test item!");
        //}
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
            Debug.Log("1");
            availableGames[gameName] = address;
            GameObject newGameItem = Instantiate(GameListItemPrefab, AvailableGamesScrollView.content);
            Debug.Log($"2 {newGameItem.name}");
            RectTransform rectTransform = newGameItem.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(AvailableGamesScrollView.viewport.rect.width, 50); // Ширина = ширине Viewport, высота = 50

            TMP_Text gameText = newGameItem.GetComponentInChildren<TMP_Text>();
            gameText.text = gameName;
            Debug.Log($"3 {gameName}; {gameText}");
            Button button = newGameItem.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => SelectGame(gameName, address));
            Debug.Log($"4 {button.name}");
            LayoutRebuilder.ForceRebuildLayoutImmediate(AvailableGamesScrollView.content.GetComponent<RectTransform>());
            Debug.Log("5");
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
            Debug.Log("NetworkManager ended its work");
        }
    }
}

