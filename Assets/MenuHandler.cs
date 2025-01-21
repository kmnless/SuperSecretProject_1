using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Button captureButton;
    [SerializeField] private float captureRadius = 3.0f;
    private Transform playerTransform;
    private Transform flagTransform;
    private FlagHandler currentFlag;
    private OutpostHandler currentOutpost;
    private int playerId;

    private void Awake()
    {
        captureButton.interactable = false;
    }
    private void Update()
    {
        if (playerTransform == null || flagTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerHandlerScript>()?.transform;
            flagTransform = transform;
        }

        if (playerTransform != null && flagTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, flagTransform.position);
            captureButton.interactable = distance <= captureRadius;
        }
    }

    private void Start()
    {
        menuUI.SetActive(false);
        playerId = GlobalVariableHandler.Instance.MyIndex.Value;
    }

    public void Open(FlagHandler flag)
    {
        currentFlag = flag;
        menuUI.SetActive(true);
    }
    public void Open(OutpostHandler outpost)
    {
        currentOutpost = outpost;
        menuUI.SetActive(true);
    }

    //public void CaptureFlag()
    //{
    //    int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;
    //    int flagId = GetComponentInParent<FlagHandler>().flagId;

    //    if (playerId >= 0)
    //    {
    //        CaptureHandler.SendRequestCaptureFlag(flagId, playerId);
    //    }
    //}
    //public void CaptureOutpost()
    //{
    //    int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;
    //    int outpostId = GetComponentInParent<OutpostHandler>().outpostId;

    //    if (playerId >= 0)
    //    {
    //        CaptureHandler.SendRequestCaptureOutpost(outpostId, playerId);
    //    }
    //}
    public void CaptureFlag()
    {
        int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;

        if (currentFlag != null && !currentFlag.isBeingCaptured)
        {
            CaptureHandler.SendRequestCaptureFlag(currentFlag.flagId, playerId);
        }
        Close();
    }
    public void CaptureOutpost()
    {
        int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;

        if (currentOutpost != null && !currentOutpost.isBeingCaptured)
        {
            CaptureHandler.SendRequestCaptureOutpost(currentOutpost.outpostId, playerId);
        }
        Close();
    }
    public void Close()
    {
        menuUI.SetActive(false);
        currentFlag = null;
        currentOutpost = null;
    }
}
