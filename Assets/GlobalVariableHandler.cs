using System;
using System.Net;
using UnityEngine;

public class GlobalVariableHandler : MonoBehaviour
{
    // Singleton Instance
    public static GlobalVariableHandler Instance { get; private set; }

    // Public variables to store global data
    public Color[] Colors { get; set; }
    public Texture2D[] Textures { get; set; }
    public Texture2D RoadTexture { get; set; }
    public Texture2D FlagTexture { get; set; }
    public Texture2D BaseTexture { get; set; }
    public GameObject BasePrefab { get; set; }
    public GameObject FlagPrefab { get; set; }
    public GameObject OutpostPrefab { get; set; }
    public PlayerProperty[] Players { get; set; }
    public int MyIndex { get; set; } = 0;
    public int PlayerCount { get; set; }
    public float CellSize { get; set; }
    public int FieldSizeX { get; set; }
    public int FieldSizeY { get; set; }
    public double[,] TerrainField { get; set; }
    public int[,] BuildingsField { get; set; }
    public int Waterline { get; set; }
    public int MountainLine { get; set; }
    public IPEndPoint ServerIPEndPoint { get; set; }

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
}
