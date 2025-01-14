using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PimDeWitte.UnityMainThreadDispatcher;

public class ServerDiscoverer : MonoBehaviour
{
    [SerializeField] private GameConnectionHandler connectionHandler;
    private UdpClient udpClient;
    private const int ListenPort = GlobalVariableHandler.BroadcastPort; // kostil ??

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
                //message = "{GameName}|{PlayerCount}|{MaxPlayers}|{GetLocalIPAddress()}|{BroadcastPort}"
                //Debug.Log($"Recieved message: {message}");
                string[] parts = message.Split('|');
                if (parts.Length == 5)
                {
                    string gameName = parts[0];
                    int playerCount = int.Parse(parts[1]);
                    int maxPlayers = int.Parse(parts[2]);
                    string ipAddress = parts[3];
                    string port = parts[4];
                    string address = $"{ipAddress}:{port}";

                    UnityMainThreadDispatcher.Instance().Enqueue(() => connectionHandler.AddGameToList(gameName, playerCount, maxPlayers, address));
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
