using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlagHandler : MonoBehaviour
{
    public int flagId;
    [SerializeField] private TMP_Text statusText;
    public int ownerID { get; set; } = -1;
    [SerializeField] private SpriteRenderer texture;
    [SerializeField] public int captureCost = 50;
    public float moneyEarning = 5;
    public bool isBeingCaptured = false;

    [SerializeField] GameObject sliderBox;
    [SerializeField] private Slider progressBarSlider;
    private void Start()
    {
        UpdateStatusText();
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

        statusText.text = $"Owner: {ownerName}\nEarning: {moneyEarning}\nCapture Cost: {captureCost}";
    }

    public void StartCapture()
    {
        StartCoroutine(CaptureFlagRoutine());
    }

    private IEnumerator CaptureFlagRoutine()
    {
        sliderBox.SetActive(true);
        float captureTime = 3f;
        float elapsed = 0f;

        while (elapsed < captureTime)
        {
            elapsed += Time.deltaTime;
            progressBarSlider.value = elapsed / captureTime;

            yield return null;
        }
        sliderBox.SetActive(false);
    }

}
