using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour
{
    private static ClientRpcHandler clientRpcHandler;

    public void Start()
    {
        clientRpcHandler = GameObject.Find("ClientRpcHandler").GetComponent<ClientRpcHandler>();
    }

    public static void UpgradeStrength(int playerId, int strengthDelta)
    {
        clientRpcHandler.RequestStrengthIncreaseServerRpc(playerId, strengthDelta);
    }
    public static void UpgradeSpeed(int playerId, float speedDelta)
    {
        clientRpcHandler.RequestSpeedIncreaseServerRpc(playerId, speedDelta);
    }
}
