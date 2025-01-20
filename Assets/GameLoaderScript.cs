using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class GameLoaderScript : MonoBehaviour
{
    [SerializeField] static public float spriteSize = 128.0f;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject flags;
    [SerializeField] private GameObject bases;
    [SerializeField] private GameObject outposts;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navigator;
    [SerializeField] private UIManager uiManager;
    public static List<Vector3> basePositions = new List<Vector3>();

    private void Awake()
    {
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
            StartCoroutine(BuildNavMesh());

            InitializeGameManager();

            if (ServerHandler.Instance is not null)
            {
                try
                {
                    GameManager.Instance.SpawnPlayers();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        };
    }

    private void InitializeGameManager()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        GameManager.Instance.map = this.map;
        GameManager.Instance.flags = this.flags;
        GameManager.Instance.bases = this.bases;
        GameManager.Instance.outposts = this.outposts;
        GameManager.Instance.playerPrefab = this.playerPrefab;
        GameManager.Instance.navigator = this.navigator;
        GameManager.Instance.uiManager = this.uiManager;
        GameManager.Instance.InitUI();
        GameManager.Instance.PreGameCountdown();
    }
    private IEnumerator BuildNavMesh()
    {
        yield return navigator.BuildNavMeshAsync();
        Debug.Log("NavMesh built successfully.");
    }
    private void Start()
    {
        navigator.BuildNavMeshAsync();
        navigator.UpdateNavMesh(navigator.navMeshData);
    }
}