using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject playerStatsPrefab;
    [SerializeField] private GameObject otherPlayerStatsPrefab;
    [SerializeField] private Transform playerStatsContainer;
    [SerializeField] private GameObject messagePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePlayerStats()
    {
        foreach (Transform child in playerStatsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == GlobalVariableHandler.Instance.MyIndex.Value)
            {
                PlayerProperty myPlayer = player;
                GameObject playerStats = Instantiate(playerStatsPrefab, playerStatsContainer);
                TMP_Text[] textFields = playerStats.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{myPlayer.Name} Lvl {myPlayer.Level}\t{myPlayer.CurrentXP}/{myPlayer.NeededXP}";
                textFields[1].text = $"Money: {Math.Round(myPlayer.Money)} ({myPlayer.MoneyIncome} per minute)";
                textFields[2].text = $"Diamonds: {Math.Round(myPlayer.Diamonds)} ({myPlayer.DiamondsIncome} per minute)";
                textFields[3].text = $"Strength: {myPlayer.Strength} x {Math.Round(myPlayer.StrengthMultiplier, 2)}";
                break;
            }
        }

        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == GlobalVariableHandler.Instance.MyIndex.Value)
            {
                continue;
            }

            GameObject otherPlayerStats = Instantiate(otherPlayerStatsPrefab, playerStatsContainer);
            TMP_Text[] textFields = otherPlayerStats.GetComponentsInChildren<TMP_Text>();
            textFields[0].text = $"{player.Name} Lvl {player.Level}\t{player.CurrentXP}/{player.NeededXP}";
            textFields[1].text = $"Money: {Math.Round(player.Money)}";
        }
    }
    public void DisplayVictoryScreen(string winnerName)
    {
        GameObject victoryScreen = new GameObject("VictoryScreen");
        victoryScreen.transform.SetParent(transform, false);

        TMP_Text text = victoryScreen.AddComponent<TMP_Text>();
        text.text = $"Game Over! Winner: {winnerName}";
        text.color = Color.green;
    }

    public void DisplayFlagProgressbar(int flagId)
    {
        FindFlagById(flagId).StartCapture();
    }
    public void DisplayOutpostProgressbar(int outpostId)
    {
        FindOutpostById(outpostId).StartCapture();
    }
    public void ShowMessage(string message)
    {
        GameObject messageBox = Instantiate(messagePrefab);
        messageBox.GetComponent<TMP_Text>().text = message;
        StartCoroutine(ClearMessageAfterDelay(3, messageBox));
    }
    private IEnumerator ClearMessageAfterDelay(float delay, GameObject message)
    {
        yield return new WaitForSeconds(delay);
        Destroy(message);
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

}
