using TMPro;
using UnityEngine;

public class OutpostHandler : MonoBehaviour
{
    public int outpostId;
    public int ownerID { get; set; } = -1;
    [SerializeField] private SpriteRenderer texture;

    public float DiamondEarning = 10;
    private void Start()
    {
        
    }

    public void UpdateOutpostAppearance(int playerId)
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

    public void CaptureFlag(int playerId)
    {
    }
    public void UpdateOutpostInfo(int playerId)
    {
        var text = GetComponentInChildren<TMP_Text>();
        text.text = playerId.ToString();
    }
}
