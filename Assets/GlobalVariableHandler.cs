using System;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;

public class GlobalVariableHandler : NetworkBehaviour
{
    // Singleton Instance
    public static GlobalVariableHandler Instance { get; private set; }

    // Public variables to store global data
    public string ServerName { get; set; }
    public const int BroadcastPort = 8888;
    public const string DefaultGameName = "GameName";
    public ushort GamePort = 2282;
    public Color[] Colors { get; set; }
    public Texture2D[] Textures { get; set; }
    public Texture2D RoadTexture { get; set; }
    public Texture2D FlagTexture { get; set; }
    public Texture2D BaseTexture { get; set; }
    public GameObject BasePrefab { get; set; }
    public GameObject FlagPrefab { get; set; }
    public GameObject OutpostPrefab { get; set; }
    public NetworkList<PlayerProperty> Players { get; set; } = new NetworkList<PlayerProperty>();
    public int MyIndex { get; set; } = 0;
    public int PlayerCount { get; set; }
    public float CellSize { get; set; }
    public int FieldSizeX { get; set; }
    public int FieldSizeY { get; set; }
    public double[,] TerrainField { get; set; }
    public int[,] BuildingsField { get; set; }
    public int Waterline { get; set; }
    public int MountainLine { get; set; }
    // Constants
    public const int CaptureDistance = 3;
    public const int AttacksToDefeat = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Preserve this object across scene loads
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("GlobalVariableHandler spawned and NetworkList initialized.");
        }
    }
}
