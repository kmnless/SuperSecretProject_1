using NavMeshPlus.Components;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public static float spriteSize = 128.0f;
    public int winConditionPoints = 1000;

    [Header("Prefabs and References")]
    public GameObject map;
    public GameObject flags;
    public GameObject bases;
    public GameObject outposts;
    public GameObject playerPrefab;
    public NavMeshSurface navigator;

    public static List<Vector3> basePositions = new List<Vector3>();

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
    private void BuildNavMesh()
    {
        navigator.BuildNavMeshAsync();
        navigator.UpdateNavMesh(navigator.navMeshData);
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
}