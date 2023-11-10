using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;


public class GlobalVariableHandler : MonoBehaviour
{
    static public Color[] colors;
    static public Texture2D[] textures;
    static public Texture2D roadTexture;
    static public Texture2D flagTexture;
    static public Texture2D baseTexture;
    static public GameObject basePrefab;
    static public GameObject flagPrefab;
    static public GameObject outpostPrefab;
    static public PlayerProperty[] players;
    static public String[] playerNames;
    static public int myIndex = 0; // Sejcas prosto pro debugging, ptom nuzhno budet prosit server poluchit eto pole;
    static public int playerCount;
    static public float cellSize;
    static public int fieldSizeX;
    static public int fieldSizeY;
    static public double[,] terrainField;
    static public int[,] buldingsField;
    static public int waterline;
    static public int montainLine;
    static public IPEndPoint serverIPEndPoint;
    public const int captureDistance=3;
}
