using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectConnectHandler : MonoBehaviour
{
    [SerializeField] private GameObject directConnectPopup;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button directConnectButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button cancelButton;

    public static string Ip { get; private set; }
    public static string Port { get; private set; }
    public static string Name { get; private set; }

    private void Start()
    {
        directConnectButton.onClick.AddListener(ShowPopup);
        connectButton.onClick.AddListener(OnConnectButtonClicked);
        connectButton.interactable = false;
        cancelButton.onClick.AddListener(HidePopup);
        cancelButton.interactable = true;
    }
    private void Update()
    {
        if (!string.IsNullOrEmpty(nameInputField.text) && !string.IsNullOrEmpty(portInputField.text) && !string.IsNullOrEmpty(ipInputField.text))
            connectButton.interactable = true;
        else connectButton.interactable = false;
    }

    public void ShowPopup()
    {
        directConnectPopup.SetActive(true);
    }

    private void HidePopup()
    {
        directConnectPopup.SetActive(false);
    }

    private void OnConnectButtonClicked()
    {
        string ip = ipInputField.text.Trim();
        string portText = portInputField.text.Trim();
        string name = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(portText) || string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("IP or Port or Name cannot be empty.");
            return;
        }

        Ip = ip;
        Port = portText;
        Name = name;

        GameConnectionHandler.ConnectDirect();
    }
}
