using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientRpcHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private Button ready;
    public NetworkMediator networkMediator;
    private void Start()
    {
        ready.onClick.AddListener(() => {
            SendRequestToServer(NetworkManager.Singleton.LocalClientId);
        });
    }
    private void UpdateMediatorReference()
    {
        var mediator = FindObjectOfType<NetworkMediator>();
        if (mediator != null)
        {
            Debug.Log("NetworkMediator reference updated.");
        }
        else
        {
            Debug.LogError("NetworkMediator not found.");
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

    public void SendRequestToServer(ulong message)
    {
        if (networkMediator != null)
        {
            networkMediator.RequestServerActionServerRpc(message);
        }
        else
        {
            Debug.LogError("NetworkMediator not found");
            UpdateMediatorReference();
        }
    }

    public override void OnNetworkSpawn()
    {
        UpdateMediatorReference();
    }

}
