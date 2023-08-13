using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameLoaderScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float spriteSize = 128.0f;
    [SerializeField] private GameObject map;
    private void Awake()
    {
        MapScript.CreateSpriteMap(GlobalVariableHandler.fieldSizeX,GlobalVariableHandler.fieldSizeY,GlobalVariableHandler.terrainField,GlobalVariableHandler.buldingsField,spriteSize,map);
    }
}
