using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject playerStatsPrefab;
    [SerializeField] private GameObject otherPlayerStatsPrefab;
    [SerializeField] private Transform playerStatsContainer;
    
    public void UpdatePlayerStats(NetworkList<PlayerProperty> players, int myIndex)
    {
        foreach (Transform child in playerStatsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in players)
        {
            if (player.Id == myIndex)
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
                GameObject otherPlayerStats = Instantiate(playerStatsPrefab, playerStatsContainer);
                TMP_Text[] textFields = otherPlayerStats.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{player.Name} Lvl {player.Level}\t{player.CurrentXP}/{player.NeededXP}";
                textFields[1].text = $"Money: {player.Money}";
            }
        }
    }
}
