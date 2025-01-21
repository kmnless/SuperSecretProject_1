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
        if (playerId < 0)
        {
            Debug.LogError("Invalid player ID.");
            return;
        }

        if (TryGetComponent(out Renderer renderer))
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
                renderer.material.color = player.Value.Color;
            }
            else
            {
                Debug.LogError($"Player with ID {playerId} not found.");
            }
        }
        else
        {
            Debug.LogError("Renderer component not found on FlagHandler.");
        }
    }
    public void UpdateFlagInfo(PlayerProperty player)
    {

    }
}
