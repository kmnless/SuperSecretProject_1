using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameLoaderScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] static public float spriteSize = 128.0f;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject flags;
    [SerializeField] private GameObject bases;
    [SerializeField] private GameObject outposts;
    [SerializeField] private PlayerHandlerScript playerHandler;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navigator;
    private void Awake()
    {
        try
        {
            MapScript.CreateSpriteMap(GlobalVariableHandler.fieldSizeX, GlobalVariableHandler.fieldSizeY, GlobalVariableHandler.terrainField, GlobalVariableHandler.buldingsField, spriteSize, map);
            MapScript.CreateEntities(GlobalVariableHandler.fieldSizeX, GlobalVariableHandler.fieldSizeY, GlobalVariableHandler.buldingsField, spriteSize, bases, flags, outposts);
        }
        catch(Exception ex) 
        {
            Debug.LogException(ex);
        };

    }
    private void Start()
    {
        navigator.BuildNavMeshAsync();

    }
}
