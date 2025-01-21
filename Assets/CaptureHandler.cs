using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureHandler : MonoBehaviour
{
    private static ClientRpcHandler clientRpcHandler;

    public void Start()
    {
        clientRpcHandler = GameObject.Find("ClientRpcHandler").GetComponent<ClientRpcHandler>();
    }
    public static void SendRequestCaptureFlag(int flagId, int playerId)
    {
        Debug.Log("handler");
        clientRpcHandler.RequestCaptureFlagServerRpc(flagId, playerId);
    }
    public static void SendRequestCaptureOutpost(int outpostId, int playerId)
    {
        clientRpcHandler.RequestCaptureOutpostServerRpc(outpostId, playerId);
    }
}

