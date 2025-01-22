using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenuHandler : MonoBehaviour
{
    [SerializeField] private Button upgradeStrengthButton;
    [SerializeField] private Button upgradeSpeedButton;
    [SerializeField] private GameObject menuUI;
    private static Transform playerTransform;
    [SerializeField] private float interactRadius = 2f;
    private BaseHandler handler;
    private void Start()
    {
        handler = GetComponentInParent<BaseHandler>();
        if (handler != null )
        {
            Debug.LogError("No Base Handler");
        }
        if (handler.Id != GlobalVariableHandler.Instance.MyIndex)
        {
            Destroy(gameObject);
        }
        upgradeStrengthButton.onClick.AddListener(OnUpgradeStrengthClicked);
        upgradeSpeedButton.onClick.AddListener(OnUpgradeSpeedClicked);
    }

    private void OnUpgradeStrengthClicked()
    {
        UpgradeHandler.UpgradeStrength(handler.OwnerId, handler.StrengthBonus);
        handler.StrengthCost += handler.StrengthCostIncrease;
    }

    private void OnUpgradeSpeedClicked()
    {
        UpgradeHandler.UpgradeSpeed(handler.OwnerId, handler.SpeedBonus);
        handler.SpeedCost += handler.SpeedCostIncrease;
    }
    public static void SetCurrentPlayer(Transform playerTransform)
    {
        BaseMenuHandler.playerTransform = playerTransform;
    }

    public void Open()
    {
        menuUI.SetActive(true);
    }
    public void Close()
    {
        menuUI.SetActive(false);
    }
}
