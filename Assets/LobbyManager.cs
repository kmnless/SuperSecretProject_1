using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Text chatBox;
    [SerializeField] private InputField chatInput;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Button readyButton;
    private Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();
    private Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        AddPlayerToList(clientId, "Player " + clientId);
        UpdatePlayerListUI();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        RemovePlayerFromList(clientId);
        UpdatePlayerListUI();
    }

    private void AddPlayerToList(ulong clientId, string playerName)
    {
        playerNames[clientId] = playerName;
        playerReadyStatus[clientId] = false;
    }

    private void RemovePlayerFromList(ulong clientId)
    {
        playerNames.Remove(clientId);
        playerReadyStatus.Remove(clientId);
    }

    private void UpdatePlayerListUI()
    {
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in playerNames)
        {
            var listItem = Instantiate(playerListItemPrefab, playerListContainer);
            listItem.GetComponent<Text>().text = player.Value + (playerReadyStatus[player.Key] ? " (Ready)" : "");
        }
    }
    
    public void SetReadyStatus()
    {
        playerReadyStatus[NetworkManager.Singleton.LocalClientId] = true;
        UpdatePlayerListUI();
        CheckIfAllReady();
    }

    private void CheckIfAllReady()
    {
        if (IsHost && playerReadyStatus.Values.All(status => status))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
