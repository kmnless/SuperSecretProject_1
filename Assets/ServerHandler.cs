using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerHandler : MonoBehaviour
{
    private static int MaxConnections;
    private static int PlayerCount;
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
            Debug.Log("ConnectionApprovalCallback registered.");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }
    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("OnConnectionApproval called.");

        if (request.Payload == null || request.Payload.Length == 0)
        {
            Debug.LogError("ConnectionData is null or empty.");
            response.Approved = false;
            return;
        }

        string nickname = System.Text.Encoding.UTF8.GetString(request.Payload);

        Debug.Log($"Client {request.ClientNetworkId} is trying to connect with nickname: {nickname}");

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxConnections)
        {
            Debug.LogWarning($"Connection rejected for client {request.ClientNetworkId}. Server is full.");
            response.Approved = false;
            return;
        }

        Debug.Log($"{nickname}, {request.ClientNetworkId}");

        var player = new PlayerProperty(nickname, (int)request.ClientNetworkId);
        GlobalVariableHandler.Instance.Players.Add(player);

        // Одобряем подключение
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

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

        PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        ServerBroadcaster.PlayerCount = PlayerCount;

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
    }
}
