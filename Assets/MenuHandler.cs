using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    private FlagHandler currentFlag;
    private OutpostHandler currentOutpost;
    private int playerId;

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
    public void Capture()
    {
        int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;

        if (currentFlag != null && !currentFlag.isBeingCaptured)
        {
            CaptureHandler.SendRequestCaptureFlag(currentFlag.flagId, playerId);
        }
        else if (currentOutpost != null && !currentOutpost.isBeingCaptured)
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
