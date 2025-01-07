using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class ServerHandler : MonoBehaviour
{
    private static int MaxConnections;
    private static int PlayerCount;
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > MaxConnections)
        {
            Debug.Log($"Connection rejected for client {clientId}. Server is full.");
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
        else
        {
            Debug.Log($"Client {clientId} connected. Total clients: {NetworkManager.Singleton.ConnectedClients.Count}");
            PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
            ServerBroadcaster.PlayerCount = PlayerCount;
        }
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
