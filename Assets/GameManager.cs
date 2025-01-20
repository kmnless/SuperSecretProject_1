using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public static float spriteSize = 128.0f;
    public int winConditionPoints = 1000;

    [Header("Prefabs and References")]
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject flags;
    [SerializeField] private GameObject bases;
    [SerializeField] private GameObject outposts;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navigator;

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

    void InitializeSceneObjects()
    {
        if (map == null) map = GameObject.Find("Map");
        if (flags == null) flags = GameObject.Find("Flags");
        if (bases == null) bases = GameObject.Find("Bases");
        if (outposts == null) outposts = GameObject.Find("Outposts");
        if (navigator == null) navigator = FindObjectOfType<NavMeshPlus.Components.NavMeshSurface>();

        if (map == null || flags == null || bases == null || outposts == null || navigator == null)
        {
            Debug.LogError("One or more required objects could not be found in the scene.");
        }

    }

    public void InitializeMap()
    {
        InitializeSceneObjects();
        try
        {
            MapScript.CreateSpriteMap(GlobalVariableHandler.Instance.FieldSizeX,
                GlobalVariableHandler.Instance.FieldSizeY,
                GlobalVariableHandler.Instance.TerrainField,
                GlobalVariableHandler.Instance.BuildingsField,
                spriteSize, map);

            MapScript.CreateEntities(GlobalVariableHandler.Instance.FieldSizeX,
                GlobalVariableHandler.Instance.FieldSizeY,
                GlobalVariableHandler.Instance.BuildingsField,
                spriteSize, bases, flags, outposts);

            BuildNavMesh();
            SpawnPlayers();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing map: {ex}");
        }
    }

    private void BuildNavMesh()
    {
        navigator.BuildNavMeshAsync();
        navigator.UpdateNavMesh(navigator.navMeshData);
    }

    private void SpawnPlayers()
    {
        if (!IsServer)
        {
            Debug.LogError("Player spawn can only be initiated by the server.");
            return;
        }

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