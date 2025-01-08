using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerHandler : MonoBehaviour
{
    private static int MaxConnections = 1;
    private static int PlayerCount;
    private static PlayerProperty pl;
    private bool IsFirst = true;
    private void Start()
    {
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

        if (IsFirst)
            pl = player;
        else
            GlobalVariableHandler.Instance.Players.Add(player);

        IsFirst = false;
        response.Approved = true;
        //response.CreatePlayerObject = true; // Создавать объект игрока, если используется PlayerPrefab
        //response.Position = Vector3.zero; // Начальная позиция, если нужно
        //response.Rotation = Quaternion.identity;
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

        PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ServerBroadcaster.PlayerCount = PlayerCount;

        if(!NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected. Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");
        PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ServerBroadcaster.PlayerCount = PlayerCount;
    }

    public static void RefreshPlayerCount()
    {
        MaxConnections = GlobalVariableHandler.Instance.PlayerCount;
        GlobalVariableHandler.Instance.Players.Add(pl);
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

}
