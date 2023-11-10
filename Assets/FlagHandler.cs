using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public int ownerID {get;set;}  = -1;
    public int unitCount{get;set;} = 0;
    public int XPyield{get;set;}
    [SerializeField] private SpriteRenderer texture;
    void Start()
    {
        transform.GetComponentInChildren<Canvas>(true).worldCamera=GameObject.Find("Camera").GetComponent<Camera>();   
    }
    public void Capture(int id)
    {
        ownerID = id;
        texture.color = GlobalVariableHandler.colors[id];
    }

    void Update()
    {
        
    }
}
