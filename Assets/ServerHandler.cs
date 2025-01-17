using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ServerHandler : MonoBehaviour
{
    public static ServerHandler Instance;

    private static int MaxConnections = 1;
    private static int PlayerCount;
    private static int ColorCount = 0;
    private static PlayerProperty pl;
    private bool IsFirst = true;

    private List<PlayerReadyStatus> PlayersReadyList;

    public TMP_Text countdownText;
    private Coroutine countdownCoroutine;
    private bool isCountdownActive = false;
    [SerializeField] private float countdownTime = 3f;

    [SerializeField] private NetworkMediator networkMediator;
    private ClientRpcHandler clientRpcHandler;

    [SerializeField] public static GameObject playerPrefab;
    [SerializeField] public static GameObject clientPrefab;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;

        playerPrefab = Resources.Load<GameObject>("Prefabs/PlayerPrefab");
        clientPrefab = Resources.Load<GameObject>("Prefabs/ClientPrefab");

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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
            player.Color = GlobalVariableHandler.Instance.Colors[ColorCount++];
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

        clientRpcHandler.SetMyIndexClientRpc(clientId, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        });

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
        ServerBroadcaster.PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ColorCount--;
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
        clientRpcHandler.RequestClientSetText(playerListTextContent);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            countdownText = GameObject.Find("Countdown").GetComponent<TMP_Text>();

            clientRpcHandler = GameObject.Find("ClientRpcHandler").GetComponent<ClientRpcHandler>();

            if (NetworkManager.Singleton.IsServer && FindObjectOfType<NetworkMediator>() == null)
            {
                var mediatorInstance = Instantiate(networkMediator);
                mediatorInstance.GetComponent<NetworkObject>().Spawn(true);
                DontDestroyOnLoad(mediatorInstance);
            }
            clientRpcHandler.networkMediator = FindObjectOfType<NetworkMediator>();
        }
    }
    public void SetPlayerReadyServerRpc(ulong clientId)
    {
        for(int i = 0; i < PlayersReadyList.Count; i++)
        {
            if (PlayersReadyList[i].Id == (int)clientId)
            {
                var status = PlayersReadyList[i];

                if (status.IsReady)
                    status.IsReady = false;
                else
                    status.IsReady = true;

                PlayersReadyList[i] = status;
                break;
            }
        }

        UpdatePlayerListUI();

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
            //countdownText.text = $"{Mathf.CeilToInt(t)}";
            clientRpcHandler.RequestClientSetCountdown(Mathf.CeilToInt(t).ToString());
            yield return new WaitForSeconds(1f);
        }

        clientRpcHandler.RequestClientSetCountdown("Starting game");
        StartGame();
    }
    private void StartGame()
    {
        Debug.Log("Starting game...");
        GlobalVariableHandler.Instance.SyncAllFieldsServerRpc();
        NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public static class PlayerSpawner
    {
        public static void SpawnPlayers(List<Vector3> basePositions)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogError("SpawnPlayers can only be called on the server.");
                return;
            }

            for (int i = 0; i < basePositions.Count; i++)
            {
                Vector3 spawnPosition = basePositions[i];
                if (i == GlobalVariableHandler.Instance.MyIndex)            // idk, here might be a problem
                {
                    var playerObject = GameObject.Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                    playerObject.transform.rotation = Quaternion.identity;
                    playerObject.name = $"Player{i}";
                    Debug.Log($"Player{i} spawned");
                    playerObject.GetComponent<NetworkObject>().SpawnWithOwnership((ulong)i);
                    Debug.Log($"Player{i} network spawned");

                    PlayerHandlerScript.player = playerObject;
                }
                else
                {
                    var playerObject = GameObject.Instantiate(clientPrefab, spawnPosition, Quaternion.identity);
                    playerObject.transform.rotation = Quaternion.identity;
                    playerObject.name = $"Player{i}";
                    Debug.Log($"Client{i} spawned");
                    playerObject.GetComponent<NetworkObject>().SpawnWithOwnership((ulong)i);
                    Debug.Log($"Client{i} network spawned");
                }
                Debug.Log($"Player {i} spawned at {spawnPosition}");
            }
        }
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
        pl.Color = GlobalVariableHandler.Instance.Colors[ColorCount++];
        GlobalVariableHandler.Instance.MyIndex = pl.Id;
        GlobalVariableHandler.Instance.Players.Add(pl);             // kostil??
    }

}
