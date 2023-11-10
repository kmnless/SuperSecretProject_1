using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Close()
    {
        transform.gameObject.SetActive(false);
    }
    public void Open()
    {
        transform.gameObject.SetActive(true);
    }
    
    public void Capture()
    {
        if(transform.parent.GetComponent<FlagScript>().power<1)
        {
            Debug.Log("F");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
