using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerBroadcaster : MonoBehaviour
{
    private UdpClient udpClient;
    private const int BroadcastPort = 8888;
    private const string GameName = "MyStrategyGame";

    private void Start()
    {
        udpClient = new UdpClient { EnableBroadcast = true };
        Task.Run(SendBroadcasts);
    }

    private async Task SendBroadcasts()
    {
        while (true)
        {
            string message = $"{GameName}|{GetLocalIPAddress()}|{BroadcastPort}";
            byte[] data = Encoding.UTF8.GetBytes(message);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
            udpClient.Send(data, data.Length, endpoint);

            await Task.Delay(1000);
        }
    }

    private string GetLocalIPAddress()
    {
        foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
                return address.ToString();
        }
        return "127.0.0.1";
    }

    private void OnDestroy()
    {
        udpClient?.Close();
    }
}
