using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    public int flagId;
    public int ownerID { get; set; } = -1;
    [SerializeField] private SpriteRenderer texture;
    [SerializeField] public int captureCost = 50;

    public float MoneyEarning = 5;
    private void Start()
    {
        
    }

    public void UpdateFlagAppearance(int playerId)
    {
        PlayerProperty? player = null;
        foreach (var p in GlobalVariableHandler.Instance.Players)
        {
            if (p.Id == playerId)
            {
                player = p;
                break;
            }
        }

        if (player != null)
        {
            texture.color = player.Value.Color;
        }
        else
        {
            Debug.LogError($"Player with ID {playerId} not found.");
        }
    }
    public void SetOwner(int playerId)
    {
        if (ownerID == playerId) return;
        ownerID = playerId;
    }
    public void UpdateFlagInfo(int playerId)
    {

    }
}
