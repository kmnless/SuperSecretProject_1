using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    public int ownerID { get; set; } = -1;
    [SerializeField] private SpriteRenderer texture;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterFlag(this);
        }
        else
        {
            Debug.LogError("GameManager is not initialized.");
        }
    }

    public void UpdateFlagAppearance(int playerId)
    {
        ownerID = playerId;
        texture.color = GlobalVariableHandler.Instance.Colors[playerId];
    }
    public void UpdateFlagInfo(PlayerProperty player)
    {

    }
}
