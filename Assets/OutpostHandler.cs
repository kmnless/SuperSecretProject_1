using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutpostHandler : MonoBehaviour
{
    public int outpostId;
    public int ownerID { get; set; } = -1;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private SpriteRenderer texture;
    [SerializeField] public int captureCost = 50;
    public float DiamondEarning = 10;
    public bool isBeingCaptured = false;

    [SerializeField] private Slider progressBarSlider;
    private void Start()
    {
        UpdateStatusText();
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
    public void SetOwner(int playerId)
    {
        if (ownerID == playerId) return;
        ownerID = playerId;
    }
    public void UpdateStatusText()
    {
        string ownerName = "None";
        foreach (var player in GlobalVariableHandler.Instance.Players)
        {
            if (player.Id == ownerID)
            {
                ownerName = player.Name.ToString();
                break;
            }
        }

        statusText.text = $"Owner: {ownerName}\nDiamond Earning: {DiamondEarning}\nCapture Cost: {captureCost}";
    }
    public void StartCapture()
    {
        StartCoroutine(CaptureFlagRoutine());
    }
    private IEnumerator CaptureFlagRoutine()
    {
        progressBarSlider.enabled = true;
        float captureTime = 3f;
        float elapsed = 0f;

        while (elapsed < captureTime)
        {
            elapsed += Time.deltaTime;
            progressBarSlider.value = elapsed / captureTime;

            yield return null;
        }
        progressBarSlider.enabled = false;
    }
}
