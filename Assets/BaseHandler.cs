using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BaseHandler : MonoBehaviour
{
    public int Id { get; set; }
    public int OwnerId { get; set; }

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Camera cam;

    [SerializeField] public int StrengthBonus = 5;
    [SerializeField] public int StrengthCost = 5;
    [SerializeField] public int StrengthCostIncrease = 5;
    [SerializeField] public float SpeedBonus = 0.1f;
    [SerializeField] public float SpeedCost = 10;
    [SerializeField] public float SpeedCostIncrease = 10;

    void Start()
    {
        foreach(var p in GlobalVariableHandler.Instance.Players)
        {
            if (p.Id == OwnerId)
                spriteRenderer.color = p.Color;
        }
        cam = GameObject.Find("Camera").GetComponent<Camera>(); 
        foreach(Canvas child in transform.GetComponentsInChildren<Canvas>(true))
        {
            child.worldCamera = cam;
        }
    }
    public void UpgradeStat(string statType, int playerId)
    {
        PlayerProperty? pl = null;

        foreach (var p in GlobalVariableHandler.Instance.Players)
        {
            if (p.Id == playerId)
            {
                pl = p;
            }
        }

    }

    void Update()
    {
        
    }
}
