using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerDiscoverer : MonoBehaviour
{
    [SerializeField] private GameConnectionHandler connectionHandler;
    private UdpClient udpClient;
    private const int ListenPort = 8888;

    private void Start()
    {
        udpClient = new UdpClient(ListenPort);
        Task.Run(ReceiveBroadcasts);
    }

    private async Task ReceiveBroadcasts()
    {
        while (true)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                Debug.Log($"Received broadcast: {message}");

                string[] parts = message.Split('|');
                if (parts.Length == 3)
                {
                    string gameName = parts[0];
                    string ipAddress = parts[1];
                    string port = parts[2];
                    string address = $"{ipAddress}:{port}";

                    Debug.Log($"Calling AddGameToList with: {gameName}, {address}");
                    connectionHandler.AddGameToList(gameName, address);
                }
                else
                {
                    Debug.LogWarning($"Invalid broadcast format: {message}");
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError($"UDP Receive Error: {ex.Message}");
            }
        }
    }


    private void OnDestroy()
    {
        udpClient?.Close();
    }
}
