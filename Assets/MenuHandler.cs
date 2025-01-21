using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    private FlagHandler currentFlag;
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

    public void Capture()
    {
        int playerId = GlobalVariableHandler.Instance.MyIndex ?? -1;
        int flagId = GetComponentInParent<FlagHandler>().flagId;

        if (playerId >= 0)
        {
            CaptureHandler.SendRequestCaptureFlag(flagId, playerId);
        }
    }

    public void Close()
    {
        menuUI.SetActive(false);
    }
}
