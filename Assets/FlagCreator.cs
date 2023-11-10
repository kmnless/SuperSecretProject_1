using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponentInChildren<Canvas>(true).worldCamera=GameObject.Find("Camera").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
