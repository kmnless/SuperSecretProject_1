using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct playerState 
{
    Color baseColor;
    int[] collectedFlags;
    int power;
    int level;
    Vector2 pos;
}
public class GlobalVariableHandler : MonoBehaviour
{
    static public Texture2D[] textures;
    static public Texture2D roadTexture;
    static public Texture2D flagTexture;
    static public Texture2D baseTexture;
    static public int playerCount;
    static public playerState[] playersState;
    static public float cellSize;
    static public int fieldSizeX;
    static public int fieldSizeY;
    static public double[,] terrainField;
    static public int[,] buldingsField;
    
}
