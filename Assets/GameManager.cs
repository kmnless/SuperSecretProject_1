using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                float income = 50;
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
            CheckWinCondition();
            UpdateUIForAllPlayers();
        }
    }
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
        clientRpcHandler.NotifyFlagAnimationClientRpc(flag.flagId);
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
        clientRpcHandler.NotifyOutpostAnimationClientRpc(outpost.outpostId);
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
            playerObject.GetComponent<PlayerHandlerScript>().SpawnPoint = spawnPosition;
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
    private BaseHandler FindBaseByOwner(int ownerId)
    {
        BaseHandler[] bases = FindObjectsOfType<BaseHandler>();
        foreach (var b in bases)
        {
            if (b.OwnerId == ownerId)
            {
                return b;
            }
        }
        return null;
    }
    private PlayerProperty? FindPlayerProperty(int playerId)
    {
        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == playerId)
            {
                return GlobalVariableHandler.Instance.Players[i];
            }
        }
        return null;
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
    public void HandleStrengthIncrease(int playerId, int strengthDelta)
    {
        var b = FindBaseByOwner(playerId);
        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == playerId)
            {
                var p = GlobalVariableHandler.Instance.Players[i];
                if (p.Diamonds >= b.StrengthCost)
                {
                    p.Strength += strengthDelta;
                    p.Diamonds -= b.StrengthCost;
                    b.UpgradeStrength();
                }
                GlobalVariableHandler.Instance.Players[i] = p;
                return;
            }
        }
    }
    public void HandleSpeedIncrease(int playerId, float speedDelta)
    {
        var b = FindBaseByOwner(playerId);

        for (int i = 0; i < GlobalVariableHandler.Instance.Players.Count; i++)
        {
            if (GlobalVariableHandler.Instance.Players[i].Id == playerId)
            {
                var p = GlobalVariableHandler.Instance.Players[i];
                if (p.Diamonds >= b.SpeedCost)
                {
                    p.MoveSpeed += speedDelta;
                    p.Diamonds -= b.SpeedCost;
                    b.UpgradeSpeed();
                }
                GlobalVariableHandler.Instance.Players[i] = p;
                return;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartBattleServerRpc(ulong attackerId, ulong defenderId)
    {
        PlayerHandlerScript attacker = GetPlayerById(attackerId);
        PlayerHandlerScript defender = GetPlayerById(defenderId);

        if (attacker == null || defender == null)
        {
            Debug.LogError("Players not found");
            return;
        }

        var p1 = FindPlayerProperty((int)attacker.OwnerClientId).Value;
        var p2 = FindPlayerProperty((int)defender.OwnerClientId).Value;

        int attackerStrength = (int)Math.Round(p1.Strength * p1.StrengthMultiplier);
        int defenderStrength = (int)Math.Round(p2.Strength * p2.StrengthMultiplier);

        int attackerDiceRolls = attackerStrength > defenderStrength ? 2 : 1;
        int defenderDiceRolls = defenderStrength > attackerStrength ? 2 : 1;

        int attackerRoll = RollDice(attackerDiceRolls);
        int defenderRoll = RollDice(defenderDiceRolls);


        Debug.Log($"Battle: {attacker.playerName} ({attackerRoll}) vs {defender.playerName} ({defenderRoll})");

        if (attackerRoll > defenderRoll)
            HandleVictory(attacker, defender, attackerRoll, defenderRoll);
        else if (defenderRoll > attackerRoll)
            HandleVictory(defender, attacker, defenderRoll, attackerRoll);
        else
            HandleDraw(attacker, defender);
    }

    private int RollDice(int diceCount)
    {
        int total = 0;
        for (int i = 0; i < diceCount; i++)
        {
            total += UnityEngine.Random.Range(1, 7);
        }
        return total;
    }

    private void HandleVictory(PlayerHandlerScript winner, PlayerHandlerScript loser, int winnerTotal, int loserTotal)
    {
        loser.Respawn();
        NotifyBattleResultClientRpc(winner.OwnerClientId, loser.OwnerClientId, winnerTotal, loserTotal);
    }

    private void HandleDraw(PlayerHandlerScript p1, PlayerHandlerScript p2)
    {
        p1.Respawn();
        p2.Respawn();
        NotifyBattleResultClientRpc(p1.OwnerClientId, p2.OwnerClientId, 0, 0);
    }


    [ClientRpc]
    private void NotifyBattleResultClientRpc(ulong winnerId, ulong loserId, int winnerTotal, int loserTotal)
    {
        PlayerHandlerScript winner = GetPlayerById(winnerId);
        PlayerHandlerScript loser = GetPlayerById(loserId);

        if (OwnerClientId == loserId)
        {
            UIManager.Instance.ShowBattleResult($"You lose! ({winnerTotal} vs {loserTotal})", loser.transform);
            if (loser != null)
            {
                loser.Die();
            }
            else
            {
                Debug.LogError("Loser is null");
            }
        }
        else if (OwnerClientId == winnerId)
        {
            UIManager.Instance.ShowBattleResult($"You won! ({winnerTotal} vs {loserTotal})", winner.transform);
        }
    }
    [ServerRpc]
    public void StartRespawnTimerServerRpc(int playerId, int respawnTime)
    {
        StartCoroutine(RespawnTimerCoroutine(playerId, respawnTime));
    }

    private IEnumerator RespawnTimerCoroutine(int playerId, int respawnTime)
    {
        for (int i = respawnTime; i > 0; i--)
        {
            NotifyRespawnTimerClientRpc(playerId, i);
            yield return new WaitForSeconds(1f);
        }

        RespawnPlayerServerRpc(playerId);
    }

    [ClientRpc]
    private void NotifyRespawnTimerClientRpc(int playerId, int seconds)
    {
        if (GlobalVariableHandler.Instance.MyIndex == playerId)
        {
            UIManager.Instance.UpdateRespawnTimer(seconds);
        }
    }

    [ServerRpc]
    private void RespawnPlayerServerRpc(int playerId)
    {
        PlayerHandlerScript player = GetPlayerById((ulong)playerId);
        if (player != null)
        {
            player.Respawn();
        }
    }

    private PlayerHandlerScript GetPlayerById(ulong playerId)
    {
        return FindObjectsOfType<PlayerHandlerScript>().FirstOrDefault(p => p.OwnerClientId == playerId);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}