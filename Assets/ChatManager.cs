using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject chatNetworkObject;

    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject chatMessagePrefab;

    void Start()
    {
        Debug.Log($"IsClient: {IsClient}, IsServer: {IsServer}, IsHost: {IsHost}");

        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Network is active as client or host.");
        }
        else
        {
            Debug.LogWarning("Network is not active. Did you start the client?");
        }

        if (!IsClient)
        {
            Debug.LogError("Client is not connected!");
        }

        sendButton.onClick.AddListener(SendMessageToServer);
    }


    private void SendMessageToServer()
    {
        string message = messageInput.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        if (IsClient)
        {
            SendChatMessageServerRpc(GlobalVariableHandler.Instance.Players[NetworkManager.LocalClientId].Name, message);
            messageInput.text = "";
        }
        else
        {
            Debug.LogWarning("Error, server probably isn't started right");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string playerName, string message)
    {
        string formattedMessage = $"playerName: {message}";
        UpdateChatClientRpc(formattedMessage);
    }

    [ClientRpc]
    private void UpdateChatClientRpc(string message)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, chatContent);
        newMessage.GetComponent<TMP_Text>().text = message;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(newMessage.GetComponent<RectTransform>());
        RectTransform transform = chatContent.GetComponent<RectTransform>();

        transform.sizeDelta = new Vector2(transform.sizeDelta.x, transform.sizeDelta.y + 60);
        transform.anchoredPosition = new Vector2(0, 0);
    }


}
