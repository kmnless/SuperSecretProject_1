using Unity.Netcode;
using UnityEngine;

public class NetworkMediator : NetworkBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestServerActionServerRpc(ulong message, ServerRpcParams rpcParams = default)
    {
        ServerHandler.Instance?.SetPlayerReadyServerRpc(message);
    }
}
