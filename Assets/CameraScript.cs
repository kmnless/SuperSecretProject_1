using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private Transform target;
    private bool isFollowing = true;
    void Start()
    {
        target = GameObject.Find("Player").transform;
        transform.position = target.position + new Vector3(0, 0, -10);
    }
    void unBind()
    {
        isFollowing = false;
    }
    void bind()
    {
        isFollowing = true;
    }
    void Update()
    {
        if (isFollowing)
        {
            transform.position = target.position + new Vector3(0,0,-10);
        }
    }
}
