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
    private FlagHandler FindFlagById(int flagId)
    {
        FlagHandler[] flags = FindObjectsOfType<FlagHandler>();
        foreach (var flag in flags)
        {
            if (flag.flagId == flagId)
            {
                return flag;
            }
        }
        return null;
    }
    private OutpostHandler FindOutpostById(int outpostId)
    {
        OutpostHandler[] outposts = FindObjectsOfType<OutpostHandler>();
        foreach (var o in outposts)
        {
            if (o.outpostId == outpostId)
            {
                return o;
            }
        }
        return null;
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
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdatePlayerStats();
        }
    }
    [ClientRpc]
    public void NotifyFlagCapturedClientRpc(int flagId, int playerId)
    {
        FlagHandler flag = FindFlagById(flagId);
        if (flag != null)
        {
            flag.UpdateFlagAppearance(playerId);
            flag.UpdateStatusText();
        }
    }
    [ClientRpc]
    public void NotifyOutpostCapturedClientRpc(int outpostId, int playerId)
    {
        OutpostHandler outpost = FindOutpostById(outpostId);
        if (outpost != null)
        {
            outpost.UpdateOutpostAppearance(playerId);
            outpost.UpdateStatusText();
        }
    }

    [ClientRpc]
    public void NotifyFlagAnimationClientRpc(int flagId)
    {
        UIManager.Instance.DisplayFlagProgressbar(flagId);
    }
    [ClientRpc]
    public void NotifyOutpostAnimationClientRpc(int outpostId)
    {
        UIManager.Instance.DisplayOutpostProgressbar(outpostId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestCaptureFlagServerRpc(int flagId, int playerId, ServerRpcParams rpcParams = default)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleFlagCapture(flagId, playerId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestCaptureOutpostServerRpc(int outpostId, int playerId, ServerRpcParams rpcParams = default)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleOutpostCapture(outpostId, playerId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestStrengthIncreaseServerRpc(int playerId, int strengthDelta, ServerRpcParams rpcParams = default)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleStrengthIncrease(playerId, strengthDelta);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestSpeedIncreaseServerRpc(int playerId, float speedDelta, ServerRpcParams rpcParams = default)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleSpeedIncrease(playerId, speedDelta);
        }
    }

    [ClientRpc]
    public void NotifyGameEndedClientRpc(string winnerName)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.DisplayVictoryScreen(winnerName);
        }
    }


    [ClientRpc]
    public void SetMyIndexClientRpc(int index, ClientRpcParams clientRpcParams = default)
    {
        GlobalVariableHandler.Instance.MyIndex = index;
    }
}
