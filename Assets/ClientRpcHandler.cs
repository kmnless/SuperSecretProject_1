using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientRpcHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Button ready;
    public NetworkMediator networkMediator;
    
    public UIManager UIManager;
    private void Start()
    {
        ready.onClick.AddListener(() => {
            SendRequestToServer(NetworkManager.Singleton.LocalClientId);
        });
        DontDestroyOnLoad(gameObject);
    }
    private void UpdateMediatorReference()
    {
        var mediator = FindObjectOfType<NetworkMediator>();
        if (mediator != null)
        {
            networkMediator = mediator;
            Debug.Log("NetworkMediator reference updated in OnNetworkSpawn.");
        }
        else
        {
            Debug.LogError("NetworkMediator not found in OnNetworkSpawn.");
        }
    }

    [ClientRpc]
    public void SetTextClientRpc(string message, ClientRpcParams rpcParams = default)
    {
        playerList.SetText(message);
    }

    public void RequestClientSetText(string message)
    {
        SetTextClientRpc(message);
    }

    [ClientRpc]
    public void SetCountdownClientRpc(string message, ClientRpcParams rpcParams = default)
    {
        countdownText.SetText(message);
    }

    public void RequestClientSetCountdown(string message)
    {
        SetCountdownClientRpc(message);
    }

    public void SendRequestToServer(ulong message)
    {
        if (networkMediator != null)
        {
            networkMediator.RequestServerActionServerRpc(message);
        }
        else
        {
            Debug.LogError("NetworkMediator not found");
            RequestMediatorFromServerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestMediatorFromServerServerRpc(ServerRpcParams rpcParams = default)
    {
        var mediator = FindObjectOfType<NetworkMediator>();
        if (mediator != null)
        {
            mediator.NetworkObject.Spawn();
        }
    }
    [ClientRpc]
    public void StartGameClientRpc()
    {
        PlayerHandlerScript.IsStarted = true;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        UpdateMediatorReference();
        if (IsClient && !IsServer)
            GlobalVariableHandler.Instance.LoadResources();
    }

    [ClientRpc]
    public void UpdatePlayerUIClientRpc()
    {
        UIManager.UpdatePlayerStats();
    }

    [ClientRpc]
    public void InitUIManagerClientRpc(UIManager obj)
    {
        this.UIManager = obj;
    }

    [ClientRpc]
    public void SetMyIndexClientRpc(int index, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"SetMyIndexClientRpc called for client. Received index: {index}");
        GlobalVariableHandler.Instance.MyIndex = index;
    }
}
