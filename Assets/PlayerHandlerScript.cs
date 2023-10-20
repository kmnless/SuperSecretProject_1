using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandlerScript : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    public void createPlayer(Vector3 pos)
    {
        Instantiate(playerPrefab, pos + new Vector3(0,0,10), Quaternion.identity).name="Player";
    }

    public void Awake()
    {
        createPlayer(new(0,0,-10));
    }
    public void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
