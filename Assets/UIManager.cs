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
            GameObject playerStats = Instantiate(playerStatsPrefab, playerStatsContainer);

            if (player.Id == myIndex)
            {
                TMP_Text[] textFields = playerStats.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{player.Name} {player.Level} Lvl\t{player.CurrentXP}/{player.NeededXP}";
                textFields[1].text = $"Money: {player.Money}";
                textFields[2].text = $"Diamonds: {player.Diamonds}";
                textFields[2].text = $"Strength: {player.Strength} x {Math.Round(player.StrengthMultiplier, 2)}";
            }
            else
            {
                TMP_Text[] textFields = otherPlayerStatsPrefab.GetComponentsInChildren<TMP_Text>();
                textFields[0].text = $"{player.Name} {player.Level} Lvl\t{player.CurrentXP}/{player.NeededXP}"; ;
                textFields[1].text = $"Money: {player.Money}";
            }
        }
    }
}
