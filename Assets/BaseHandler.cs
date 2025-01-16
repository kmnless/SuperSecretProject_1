using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public int unitCout{get;set;} = 0;

    public int Id{get;set;}
    public string Name{get;set;}
    public int timesAttacked{get;set;}=0;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Camera cam;
    void Start()
    {
        spriteRenderer.color = Color.white;         // temp hardcode, must be player color (probably)
        cam = GameObject.Find("Camera").GetComponent<Camera>(); 
        foreach(Canvas child in transform.GetComponentsInChildren<Canvas>(true))
        {
            child.worldCamera = cam;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
