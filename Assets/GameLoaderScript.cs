using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ServerHandler;

public class GameLoaderScript : MonoBehaviour
{
    [SerializeField] static public float spriteSize = 128.0f;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject flags;
    [SerializeField] private GameObject bases;
    [SerializeField] private GameObject outposts;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navigator;

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

            navigator.BuildNavMeshAsync();
            if (ServerHandler.Instance is not null)
            {
                try
                {
                    ServerHandler.PlayerSpawner.SpawnPlayers(basePositions);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        catch(Exception ex) 
        {
            Debug.LogException(ex);
        };

    }
    //private void Start()
    //{


    //}
}
