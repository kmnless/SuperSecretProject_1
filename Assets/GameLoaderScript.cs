using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameLoaderScript : MonoBehaviour
{
    [SerializeField] static public float spriteSize = 128.0f;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject flags;
    [SerializeField] private GameObject bases;
    [SerializeField] private GameObject outposts;
    [SerializeField] private PlayerHandlerScript playerHandler;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navigator;

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


            foreach (var p in GlobalVariableHandler.Instance.Players)
            {
                Vector3 position = new Vector3(GlobalVariableHandler.Instance.CellSize / 200f, GlobalVariableHandler.Instance.CellSize / 200f, 0);

                // sample

                //var b = bases.GetComponents<GameObject>();
                //for (int i = 0; i < GlobalVariableHandler.Instance.PlayerCount; i++)
                //{
                //    if(b[i].name == p.Name)
                //        position = b[i].transform.position;
                //}
                GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
                playerObject.name = $"Player{p.Id}";
            }
            
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
