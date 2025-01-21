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
    //private void InitializeFlags()
    //{
    //    flagList.Clear();

    //    var flagHandlers = FindObjectsOfType<FlagHandler>();
    //    for (int i = 0; i < flagHandlers.Length; i++)
    //    {
    //        flagHandlers[i].Initialize(i);
    //        flagList.Add(new Flag
    //        {
    //            Index = i,
    //            OwnerId = -1,
    //            Income = flagHandlers[i].Income
    //        });
    //    }

    //    Debug.Log("Flags initialized and synchronized.");
    //}

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
    public void HandleFlagCapture(int flagId, int playerId)
    {
        FlagHandler flag = FindFlagById(flagId);
        if (flag == null)
        {
            Debug.LogError($"Flag with ID {flagId} not found.");
            return;
        }
        PlayerProperty? capturingPlayer = null;
        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == playerId)
            {
                capturingPlayer = player;
                break;
            }
        }
        if (capturingPlayer == null)
        {
            Debug.LogError($"Player with ID {playerId} not found.");
            return;
        }
        clientRpcHandler.NotifyFlagCapturedClientRpc(flagId, playerId);
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
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (GlobalVariableHandler.Instance.Players != null)
        {
        }
        Debug.Log("GameManager destroyed.");
    }
}