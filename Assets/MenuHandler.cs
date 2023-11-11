using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject player;
    PlayerHandlerScript playerScript;
    [SerializeField] private Button captureButton;
    [SerializeField] private GameObject flagVisual;
    [SerializeField] private FlagHandler handler;
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
        transform.gameObject.SetActive(true);
    }
    public void Capture()
    {
        if(handler.unitCount < playerScript.properties.Strength)
        {
            handler.Capture(playerScript.id);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(transform.position.x);
        if(transform.gameObject.activeSelf&&(flagVisual.transform.position - player.transform.position- new Vector3(0f,0f,10f)).magnitude<GlobalVariableHandler.captureDistance*GlobalVariableHandler.cellSize/100f)
        {
            captureButton.interactable = true;
        }
        else if (transform.gameObject.activeSelf)
        {
            captureButton.interactable = false;
            //Debug.Log(GlobalVariableHandler.captureDistance*GlobalVariableHandler.cellSize/100f);
            //Debug.Log((flagVisual.transform.position - player.transform.position- new Vector3(0f,0f,10f)).magnitude);
        }
    }
    void Update()
    {
        
    }
}
