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
        if (currentFlag != null && GameManager.Instance != null)
        {
            int flagIndex = GameManager.Instance.flagList.IndexOf(currentFlag);
            GameManager.Instance.CaptureFlag(flagIndex, playerId);
        }
    }

    public void Close()
    {
        menuUI.SetActive(false);
    }
}
