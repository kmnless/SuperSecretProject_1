using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Rendering.CameraUI;

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
    private bool hasWinner = false;

    private const int delayBeforeMenu = 5;

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
                float income = 0;
                float diamondIncome = 0;
                foreach (var flag in FindObjectsOfType<FlagHandler>())
                {
                    if (flag.ownerID == player.Id)
                    {
                        income += flag.moneyEarning;
                    }
                }

                foreach (var outpost in FindObjectsOfType<OutpostHandler>())
                {
                    if (outpost.ownerID == player.Id)
                    {
                        diamondIncome += outpost.DiamondEarning;
                    }
                }
                player.MoneyIncome = income;
                player.DiamondsIncome = diamondIncome;

                player.Money += (player.MoneyIncome / 60f);
                player.Diamonds += (player.DiamondsIncome / 60f);

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
    private void CheckWinCondition()
    {
        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Money >= winConditionPoints && !hasWinner)
            {
                hasWinner = true;
                AnnounceWinner(player.Id);
                break;                                          // first in list is winning. no draws
            }
        }
    }
    private void AnnounceWinner(int playerId)
    {
        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == playerId)
            {
                Debug.Log($"Player {player.Name} has won the game!");
                clientRpcHandler.NotifyGameEndedClientRpc(player.Name.ToString());
                //StartCoroutine(EndGameAndReturnToMenu());
                return;
            }
        }
        Debug.LogError("Winner not found in players list.");
    }
    private IEnumerator EndGameAndReturnToMenu()
    {
        yield return new WaitForSeconds(delayBeforeMenu);

        if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("Menu");
    }
    private PlayerHandlerScript FindPlayerHandler(int playerId)
    {
        var players = FindObjectsOfType<PlayerHandlerScript>();

        foreach (var player in players)
        {
            if (player.OwnerClientId == (ulong)playerId)
            {
                return player;
            }
        }
        return null;
    }
    private IEnumerator CaptureFlagCoroutine(PlayerProperty player, FlagHandler flag)
    {
        flag.isBeingCaptured = true;

        PlayerHandlerScript targetPlayer = FindPlayerHandler(player.Id);

        if (targetPlayer != null) targetPlayer.IsAllowedToMove = false;
        yield return new WaitForSeconds(3f);

        player.Money -= flag.captureCost;
        flag.SetOwner(player.Id);

        flag.isBeingCaptured = false;

        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == player.Id)
            {
                GlobalVariableHandler.Instance.Players[i] = player;
                break;
            }
        }

        if (targetPlayer != null) targetPlayer.IsAllowedToMove = true;

        clientRpcHandler.NotifyFlagCapturedClientRpc(flag.flagId, player.Id);
    }
    private IEnumerator CaptureOutpostCoroutine(PlayerProperty player, OutpostHandler outpost)
    {
        outpost.isBeingCaptured = true;

        PlayerHandlerScript targetPlayer = FindPlayerHandler(player.Id);

        if (targetPlayer != null) targetPlayer.IsAllowedToMove = false;
        yield return new WaitForSeconds(3f);

        player.Money -= outpost.captureCost;
        outpost.SetOwner(player.Id);

        outpost.isBeingCaptured = false;

        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == player.Id)
            {
                GlobalVariableHandler.Instance.Players[i] = player;
                break;
            }
        }

        if (targetPlayer != null) targetPlayer.IsAllowedToMove = true;

        clientRpcHandler.NotifyOutpostCapturedClientRpc(outpost.outpostId, player.Id);
    }
    public IEnumerator PreGameCountdown()
    {
        var players = FindObjectsOfType<PlayerHandlerScript>();

        foreach (var player in players)
        {
            player.IsAllowedToMove = false;
        }

        const int countdownDuration = 5;
        for (int i = countdownDuration; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }

        isStarted = true;

        foreach (var player in players)
        {
            player.IsAllowedToMove = true;
        }
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
        var p = capturingPlayer.Value;
        if (p.Money >= flag.captureCost)
        {
            StartCoroutine(CaptureFlagCoroutine(p, flag));
        }
    }
    public void HandleOutpostCapture(int outpostId, int playerId)
    {
        OutpostHandler outpost = FindOutpostById(outpostId);
        if (outpost == null)
        {
            Debug.LogError($"Flag with ID {outpostId} not found.");
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
        var p = capturingPlayer.Value;
        if (p.Money >= outpost.captureCost)
        {
            StartCoroutine(CaptureOutpostCoroutine(p, outpost));
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
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}