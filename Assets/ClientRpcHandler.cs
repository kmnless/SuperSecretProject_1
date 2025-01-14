using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientRpcHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private Button ready;
    [SerializeField] private NetworkMediator networkMediator;
    private void Start()
    {
        ready.onClick.AddListener(() => {
            SendRequestToServer(NetworkManager.Singleton.LocalClientId);
        });
        networkMediator.GetComponent<NetworkObject>().Spawn();

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
        if (networkMediator != null)
        {
            networkMediator.RequestServerActionServerRpc(message);
        }
        else
        {
            Debug.LogError("NetworkMediator not found");
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
