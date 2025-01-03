using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerDiscoverer : MonoBehaviour
{
    private UdpClient udpClient;
    private const int ListenPort = 8888;

    public GameConnectionHandler connectionHandler;

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
                string[] parts = message.Split('|');

                if (parts.Length == 3)
                {
                    string gameName = parts[0];
                    string ipAddress = parts[1];
                    int port = int.Parse(parts[2]);

                    connectionHandler.AddGameToList($"{gameName} ({ipAddress}:{port})");
                }
            }
            catch (SocketException ex)
            {
                Debug.LogWarning($"UDP Receive Error: {ex.Message}");
            }
        }
    }

    private void OnDestroy()
    {
        udpClient?.Close();
    }
}
