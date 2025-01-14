using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerHandler : MonoBehaviour
{
    private static int MaxConnections = 1;
    private static int PlayerCount;
    private static PlayerProperty pl;
    private bool IsFirst = true;

    private List<PlayerReadyStatus> PlayersReadyList;

    public TMP_Text countdownText;
    public Toggle readyToggle;
    private Coroutine countdownCoroutine;
    private bool isCountdownActive = false;
    [SerializeField] private float countdownTime = 3f;

    private ClientRpcHandler clientRpcHandler;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }
    private void Awake()
    {
        PlayersReadyList = new List<PlayerReadyStatus>();
        DontDestroyOnLoad(gameObject);
    }
    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("OnConnectionApproval called.");

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("OnConnectionApproval called on a non-server instance.");
            response.Approved = false;
            response.Reason = "Internal error";
            return;
        }

        if (request.Payload == null || request.Payload.Length == 0)
        {
            Debug.LogError("ConnectionData is null or empty.");
            response.Approved = false;
            response.Reason = "No connection data";
            return;
        }

        string nickname = System.Text.Encoding.UTF8.GetString(request.Payload);

        Debug.Log($"Client {request.ClientNetworkId} is trying to connect with nickname: {nickname}");

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxConnections)
        {
            Debug.LogWarning($"Connection rejected for client {request.ClientNetworkId}. Server is full.");
            response.Approved = false;
            response.Reason = "Server is full or another issue occurred.";
            return;
        }

        var player = new PlayerProperty(nickname, (int)request.ClientNetworkId);
        if (GlobalVariableHandler.Instance.Players == null)
        {
            Debug.LogError("Players list is not initialized!");
            return;
        }

        if (IsFirst)        // kostil?
        {
            pl = player;
        }
        else
        {
            GlobalVariableHandler.Instance.Players.Add(player);
        }

        PlayersReadyList.Add(new PlayerReadyStatus(player.Id, nickname));

        IsFirst = false;
        response.Approved = true;
    }
    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > MaxConnections)
        {
            Debug.Log($"Connection rejected for client {clientId}. Server is full.");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }
        Debug.Log($"Clientid {clientId} connected. Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");

        UpdatePlayerListUI();
        PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ServerBroadcaster.PlayerCount = PlayerCount;

        if(!NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected. Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");
        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == (int)clientId)
            {
                GlobalVariableHandler.Instance.Players.Remove(player);
                var p = new PlayerReadyStatus(player.Id, player.Name);
                PlayersReadyList.Remove(p);
            }
        }
        PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ServerBroadcaster.PlayerCount = PlayerCount;
        UpdatePlayerListUI();
    }
    private void UpdatePlayerListUI()
    {
        if (clientRpcHandler is null)
            return;
        
        string playerListTextContent = "";
        foreach (var player in PlayersReadyList)
        {
            playerListTextContent += $"{player.Nickname} - {(player.IsReady ? "Ready" : "Not Ready")}\n";
        }
        clientRpcHandler.RequestClientAction(playerListTextContent);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            countdownText = GameObject.Find("Countdown").GetComponent<TMP_Text>();
            readyToggle = GameObject.Find("ReadyToggle").GetComponent<Toggle>();
            readyToggle.onValueChanged.AddListener((bool ready) =>
            {
                SetPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId, ready);
                UpdatePlayerListUI();
            });
            clientRpcHandler = GameObject.Find("ClientRpcHandler").GetComponent<ClientRpcHandler>();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ulong clientId, bool isReady)
    {
        for(int i = 0; i < PlayersReadyList.Count; i++)
        {
            if (PlayersReadyList[i].Id == (int)clientId)
            {
                var status = PlayersReadyList[i];
                status.IsReady = isReady;
                PlayersReadyList[i] = status;
                break;
            }
        }

        if (AllPlayersReady() && !isCountdownActive)
        {
            StartCoroutine(StartCountdown());
        }
    }
    private bool AllPlayersReady()
    {
        if(PlayerCount != MaxConnections)
            return false;
        foreach (var player in PlayersReadyList)
        {
            if (!player.IsReady) return false;
        }
        return true;
    }
    private System.Collections.IEnumerator StartCountdown()
    {
        isCountdownActive = true;

        for (float t = countdownTime; t > 0; t--)
        {
            countdownText.text = $"{Mathf.CeilToInt(t)}";
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Starting game...";
        StartGame();
    }
    private void StartGame()
    {
        Debug.Log("Starting game...");
        NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public static void RefreshPlayerCount()
    {
        MaxConnections = GlobalVariableHandler.Instance.PlayerCount;
        GlobalVariableHandler.Instance.Players.Add(pl);             // kostil??
    }

}
