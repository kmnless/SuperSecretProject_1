using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProperties : MonoBehaviour
{
    // Start is called before the first frame update
    private int Id;
    private string Name;
    void Start()
    {
        SpriteRenderer spriteRenderer = transform.Find("Skin").GetComponent<SpriteRenderer>();
        spriteRenderer.color = GlobalVariableHandler.colors[Id];
    }

    public void setName(string Name)
    {
        this.Name = Name;
    }
    public void setId(int Id)
    {
        this.Id = Id;
    }
    // Update is called once per frame
}
