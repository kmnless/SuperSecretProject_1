using Generation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using Unity.Netcode;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GlobalVariableHandler : NetworkBehaviour, INetworkSerializable
{
    [Serializable]
    private class SerializedArray<T>
    {
        public int Rows;
        public int Cols;
        public T[] Data;
    }

    public static GlobalVariableHandler Instance { get; private set; }
    public string ServerName { get; set; }
    public const int BroadcastPort = 8888;
    public const string DefaultGameName = "GameName";
    public ushort GamePort = 2282;
    public Color[] Colors { get; set; }

    // Textures
    public Texture2D LowerGrassTexture { get; set; }
    public Texture2D GrassTexture { get; set; }
    public Texture2D HigherGrassTexture { get; set; }
    public Texture2D WaterTexture { get; set; }
    public Texture2D MountainTexture { get; set; }
    public Texture2D RoadTexture { get; set; }
    public Texture2D FlagTexture { get; set; }
    public Texture2D BaseTexture { get; set; }
    public GameObject BasePrefab { get; set; }
    public GameObject FlagPrefab { get; set; }
    public GameObject OutpostPrefab { get; set; }
    public Texture2D RoadHorizontalTexture;
    public Texture2D RoadVerticalTexture;
    public Texture2D RoadCornerTLTexture;
    public Texture2D RoadCornerTRTexture;
    public Texture2D RoadCornerBLTexture;
    public Texture2D RoadCornerBRTexture;
    public Texture2D RoadTUpTexture;
    public Texture2D RoadTDownTexture;
    public Texture2D RoadTLeftTexture;
    public Texture2D RoadTRightTexture;
    public Texture2D RoadCrossTexture;
    public Texture2D RoadEndDownTexture;
    public Texture2D RoadEndUpTexture;
    public Texture2D RoadEndLeftTexture;
    public Texture2D RoadEndRightTexture;

    public NetworkList<PlayerProperty> Players { get; set; } = new NetworkList<PlayerProperty>();
    public int? MyIndex { get; set; } = null;
    public int FlagCount { get; set; }
    public int OutpostCount { get; set; }
    public int MinRadius { get; set; }
    public int BanRadius { get; set; }
    public double TerrainScale { get; set; }
    public int RANGE_DENOMINATOR { get; set; }
    public int ROAD_GENERATION_COPLEXITY_DENOMINATOR { get; set; }
    public float DAMPING { get; set; }
    public float CONTRAST { get; set; }
    public float CLIP { get; set; }


    // Syncing fields
    public int PlayerCount;
    public float CellSize;
    public int FieldSizeX;
    public int FieldSizeY;
    public int Waterline;
    public int MountainLine;
    public int Seed;
    public double[,] TerrainField;
    public int[,] BuildingsField;

    // Constants
    public const int CaptureDistance = 3;
    public const int AttacksToDefeat = 3;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// Serialize int matrix
    /// </summary>
    private void SerializeArray<T>(BufferSerializer<T> serializer, ref int[,] array) where T : IReaderWriter
    {
        int rows = array?.GetLength(0) ?? 0;
        int cols = array?.GetLength(1) ?? 0;
        serializer.SerializeValue(ref rows);
        serializer.SerializeValue(ref cols);

        if (serializer.IsWriter)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int value = array[i, j];
                    serializer.SerializeValue(ref value);
                }
            }
        }
        else
        {
            array = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int value = 0;
                    serializer.SerializeValue(ref value);
                    array[i, j] = value;
                }
            }
        }
    }
    /// <summary>
    /// Serialize double matrix
    /// </summary>
    private void SerializeArray<T>(BufferSerializer<T> serializer, ref double[,] array) where T : IReaderWriter
    {
        int rows = array?.GetLength(0) ?? 0;
        int cols = array?.GetLength(1) ?? 0;
        serializer.SerializeValue(ref rows);
        serializer.SerializeValue(ref cols);

        if (serializer.IsWriter)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double value = array[i, j];
                    serializer.SerializeValue(ref value);
                }
            }
        }
        else
        {
            array = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double value = 0;
                    serializer.SerializeValue(ref value);
                    array[i, j] = value;
                }
            }
        }
    }
    /*private string SerializeToString<T>(T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        T[] flattened = new T[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                flattened[i * cols + j] = array[i, j];
            }
        }
        return JsonUtility.ToJson(new SerializedArray<T> { Rows = rows, Cols = cols, Data = flattened });
    }
    private T[,] DeserializeFromString<T>(string serializedData)
    {
        var data = JsonUtility.FromJson<SerializedArray<T>>(serializedData);
        T[,] array = new T[data.Rows, data.Cols];
        for (int i = 0; i < data.Rows; i++)
        {
            for (int j = 0; j < data.Cols; j++)
            {
                array[i, j] = data.Data[i * data.Cols + j];
            }
        }
        return array;
    }*/
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // fields
        serializer.SerializeValue(ref PlayerCount);
        serializer.SerializeValue(ref CellSize);
        serializer.SerializeValue(ref FieldSizeX);
        serializer.SerializeValue(ref FieldSizeY);
        serializer.SerializeValue(ref Waterline);
        serializer.SerializeValue(ref MountainLine);
        serializer.SerializeValue(ref Seed);

        // arrays
        SerializeArray(serializer, ref TerrainField);
        SerializeArray(serializer, ref BuildingsField);

        // colors
        if (serializer.IsWriter)
        {
            int colorCount = Colors.Length;
            serializer.SerializeValue(ref colorCount);
            foreach (var color in Colors)
            {
                Vector4 colorVec = new Vector4(color.r, color.g, color.b, color.a);
                serializer.SerializeValue(ref colorVec);
            }
        }
        else
        {
            int colorCount = 0;
            serializer.SerializeValue(ref colorCount);
            Colors = new Color[colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                Vector4 colorVec = Vector4.zero;
                serializer.SerializeValue(ref colorVec);
                Colors[i] = new Color(colorVec.x, colorVec.y, colorVec.z, colorVec.w);
            }
        }
    }
    public void LoadResources()
    {
        LowerGrassTexture = Resources.Load<Texture2D>("Textures/lower_grass");
        GrassTexture = Resources.Load<Texture2D>("Textures/grass");
        HigherGrassTexture = Resources.Load<Texture2D>("Textures/higher_grass");
        WaterTexture = Resources.Load<Texture2D>("Textures/water");
        MountainTexture = Resources.Load<Texture2D>("Textures/mountain");
        FlagTexture = Resources.Load<Texture2D>("Textures/flag");
        BaseTexture = Resources.Load<Texture2D>("Textures/castle");

        // Горизонтальная дорога
        RoadHorizontalTexture = Resources.Load<Texture2D>("Textures/Road/horizontal");
        // Вертикальная дорога
        RoadVerticalTexture = Resources.Load<Texture2D>("Textures/Road/vertical");
        // Поворот: нижний-левый
        RoadCornerBLTexture = Resources.Load<Texture2D>("Textures/Road/left_bottom");
        // Поворот: нижний-правый
        RoadCornerBRTexture = Resources.Load<Texture2D>("Textures/Road/right_bottom");
        // Поворот: верхний-левый
        RoadCornerTLTexture = Resources.Load<Texture2D>("Textures/Road/left_top");
        // Поворот: верхний-правый
        RoadCornerTRTexture = Resources.Load<Texture2D>("Textures/Road/right_top");
        // Т-образный перекресток: верх
        RoadTUpTexture = Resources.Load<Texture2D>("Textures/Road/horizontal_top");
        // Т-образный перекресток: низ
        RoadTDownTexture = Resources.Load<Texture2D>("Textures/Road/horizontal_bottom");
        // Т-образный перекресток: лево
        RoadTLeftTexture = Resources.Load<Texture2D>("Textures/Road/vertical_left");
        // Т-образный перекресток: право
        RoadTRightTexture = Resources.Load<Texture2D>("Textures/Road/vertical_right");
        // Перекресток
        RoadCrossTexture = Resources.Load<Texture2D>("Textures/Road/intersect");
        // Одиночная клетка дороги
        RoadTexture = Resources.Load<Texture2D>("Textures/Road/road");

        RoadEndDownTexture = Resources.Load<Texture2D>("Textures/Road/bottom_end");
        RoadEndUpTexture = Resources.Load<Texture2D>("Textures/Road/top_end");
        RoadEndLeftTexture = Resources.Load<Texture2D>("Textures/Road/left_end");
        RoadEndRightTexture = Resources.Load<Texture2D>("Textures/Road/right_end");

        BasePrefab = Resources.Load<GameObject>("Prefabs/BasePrefab");
        FlagPrefab = Resources.Load<GameObject>("Prefabs/FlagPrefab");
        OutpostPrefab = Resources.Load<GameObject>("Prefabs/OutpostPrefab");

        if (RoadTexture == null || FlagTexture == null || BaseTexture == null)
            Debug.LogError("One or more textures failed to load.");

        if (BasePrefab == null || FlagPrefab == null || OutpostPrefab == null)
            Debug.LogError("One or more prefabs failed to load.");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncAllFieldsServerRpc(ServerRpcParams rpcParams = default)
    {
        SyncAllFieldsClientRpc(
            PlayerCount,
            CellSize,
            FieldSizeX,
            FieldSizeY,
            Waterline,
            MountainLine,
            Seed,
            OutpostCount,
            FlagCount,
            MinRadius,
            BanRadius,
            TerrainScale,
            RANGE_DENOMINATOR,
            DAMPING,
            CONTRAST,
            CLIP,
            ROAD_GENERATION_COPLEXITY_DENOMINATOR,
            rpcParams.Receive.SenderClientId
        );
    }

    [ClientRpc]
    private void SyncAllFieldsClientRpc(int playerCount, float cellSize, int fieldSizeX, int fieldSizeY, int waterline, int mountainLine, int seed, int outpostCount, int flagCount, int minRadius, int banRadius, double terrainScale, int rANGE_DENOMINATOR, float dAMPING, float cONTRAST, float cLIP, int rOAD_GENERATION_COPLEXITY_DENOMINATOR, ulong senderClientId)
    {
        PlayerCount = playerCount;
        CellSize = cellSize;
        FieldSizeX = fieldSizeX;
        FieldSizeY = fieldSizeY;
        Waterline = waterline;
        MountainLine = mountainLine;
        Seed = seed;

        var terrain = Combiner.generatePlayField(fieldSizeX, fieldSizeY, seed, playerCount, outpostCount, flagCount, minRadius, banRadius, terrainScale, Math.Max(Math.Abs(fieldSizeX / rANGE_DENOMINATOR), Math.Abs(fieldSizeY / rANGE_DENOMINATOR)), dAMPING, cONTRAST, cLIP, rOAD_GENERATION_COPLEXITY_DENOMINATOR);

        TerrainField = terrain.Item1;
        BuildingsField = terrain.Item2;
    }

}
