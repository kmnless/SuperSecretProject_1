using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject playerStatsPrefab;
    [SerializeField] private GameObject otherPlayerStatsPrefab;
    [SerializeField] private Transform playerStatsContainer;

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
                textFields[1].text = $"Money: {myPlayer.Money} (+{myPlayer.PassiveIncome}/60 sec)";
                textFields[2].text = $"Diamonds: {myPlayer.Diamonds} (+{myPlayer.Diamonds}/60 sec)";
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
            textFields[1].text = $"Money: {player.Money}";
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

}
