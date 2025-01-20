using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private static GameObject playerStatsPrefab;
    [SerializeField] private static GameObject otherPlayerStatsPrefab;
    [SerializeField] private static Transform playerStatsContainer;

    public static void UpdatePlayerStats()
    {
        foreach (Transform child in playerStatsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == GlobalVariableHandler.Instance.MyIndex.Value)
            {
                GameObject playerStats = Instantiate(playerStatsPrefab, playerStatsContainer);
                TMP_Text[] textFields = playerStats.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{player.Name} Lvl {player.Level}\t{player.CurrentXP}/{player.NeededXP}";
                textFields[1].text = $"Money: {player.Money}";
                textFields[2].text = $"Diamonds: {player.Diamonds}";
                textFields[3].text = $"Strength: {player.Strength} x {Math.Round(player.StrengthMultiplier, 2)}";
            }
            else
            {
                GameObject otherPlayerStats = Instantiate(otherPlayerStatsPrefab, playerStatsContainer);
                TMP_Text[] textFields = otherPlayerStats.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{player.Name} Lvl {player.Level}\t{player.CurrentXP}/{player.NeededXP}";
                textFields[1].text = $"Money: {player.Money}";
            }
        }
    }
}
