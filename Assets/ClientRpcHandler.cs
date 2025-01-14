using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientRpcHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private Button ready;
    private void Start()
    {
        ready.onClick.AddListener(() => {
            SendRequestToServer(NetworkManager.Singleton.LocalClientId);
        });
    }
    [ClientRpc]
    public void SetTextClientRpc(string message, ClientRpcParams rpcParams = default)
    {
        playerList.SetText(message);
        Debug.Log(message);
    }

    public void RequestClientSetText(string message)
    {
        SetTextClientRpc(message);
    }

    public void SendRequestToServer(ulong message)
    {
        var networkManager = FindObjectOfType<NetworkMediator>();
        if (networkManager != null && !networkManager.IsServer)
        {
            networkManager.RequestServerActionServerRpc(message);
        }
        else
        {
            Debug.LogError("NetworkMediator not found or running on server!");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Debug.Log($"ClientRpcHandler spawned on client: {OwnerClientId}");
        }
    }
}
