using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    PlayerHandlerScript playerScript;
 
    [SerializeField] private Button attackButton;
    [SerializeField] private GameObject baseVisual;
    [SerializeField] private GameObject grave;
    [SerializeField] private BaseHandler handler;
    private bool dead = false;
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("PlayerHandler").GetComponent<PlayerHandlerScript>();
    }
    public void Close()
    {
        transform.gameObject.SetActive(false);
    }
    public void Open()
    {
        if(!dead)
            transform.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if(transform.gameObject.activeSelf&&(baseVisual.transform.position - player.transform.position- new Vector3(0f,0f,10f)).magnitude<GlobalVariableHandler.CaptureDistance*GlobalVariableHandler.Instance.CellSize /100f)
        {
            attackButton.interactable = true;
        }
        else if (transform.gameObject.activeSelf)
        {
            attackButton.interactable = false;
            //Debug.Log(GlobalVariableHandler.captureDistance*GlobalVariableHandler.cellSize/100f);
            //Debug.Log((flagVisual.transform.position - player.transform.position- new Vector3(0f,0f,10f)).magnitude);
        }
    }
    public void Attack()
    {
        
    }
}
