using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientRpcHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;

    [ClientRpc]
    public void PerformClientActionClientRpc(string message, ClientRpcParams rpcParams = default)
    {
        playerList.SetText(message);
        Debug.Log(message);
    }

    public void RequestClientAction(string message)
    {
        PerformClientActionClientRpc(message);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Debug.Log($"ClientRpcHandler spawned on client: {OwnerClientId}");
        }
    }
}
