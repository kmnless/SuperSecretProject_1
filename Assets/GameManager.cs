using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public ClientRpcHandler clientRpcHandler { get; set; }

    public static float spriteSize = 128.0f;
    public int winConditionPoints = 1000;

    public GameObject map;
    public GameObject flags;
    public GameObject bases;
    public GameObject outposts;
    public GameObject playerPrefab;
    public NavMeshSurface navigator;

    public bool isStarted = false;

    public static List<Vector3> basePositions = new List<Vector3>();

    [SerializeField] public List<FlagHandler> flagList;

    private Coroutine passiveIncomeCoroutine;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start() 
    {        
    }
    private void UpdateUIForAllPlayers()
    {
        if (clientRpcHandler != null)
        {
            clientRpcHandler.UpdatePlayerUIClientRpc();
        }
    }
    private void StartPassiveIncome()
    {
        passiveIncomeCoroutine = StartCoroutine(PassiveIncomeRoutine());
    }
    private IEnumerator PassiveIncomeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
            {
                var player = GlobalVariableHandler.Instance.Players[i];
                player.Money += Convert.ToInt32(player.PassiveIncome / 60f);
                GlobalVariableHandler.Instance.Players[i] = player;
            }

            UpdateUIForAllPlayers();
        }
    }
    public void InitUI()
    {
    }
    public IEnumerator PreGameCountdown()
    {
        const int countdownDuration = 5;
        for (int i = countdownDuration; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }
        isStarted = true;
        if (ServerHandler.Instance != null)
        {
            ServerHandler.Instance.StartGameClient();
        }
        StartPassiveIncome();
    }
    public void SpawnPlayers()
    {
        for (int i = 0; i < basePositions.Count; i++)
        {
            Vector3 spawnPosition = basePositions[i];
            GameObject playerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerObject.name = $"Player{i}";

            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership((ulong)i);

            Debug.Log($"Player {i} spawned at {spawnPosition}");
        }
    }
    public void RegisterFlag(FlagHandler flag)
    {
        if (flagList == null) flagList = new List<FlagHandler>();
        if (!flagList.Contains(flag))
        {
            flagList.Add(flag);
        }
    }
    public void CaptureFlag(int flagIndex, int playerId)
    {
        FlagHandler flag = flagList[flagIndex];
        if (flag.ownerID != playerId)
        {
            flag.ownerID = playerId;
            flag.UpdateFlagAppearance(playerId);
            clientRpcHandler.NotifyFlagCapturedClientRpc(flagIndex, playerId);
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (GlobalVariableHandler.Instance.Players != null)
        {
        }
        Debug.Log("GameManager destroyed.");
    }
}