using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ServerBroadcaster : MonoBehaviour
{
    private const int BroadcastPort = GlobalVariableHandler.BroadcastPort;
    public static string GameName { get; set; }
    public static int PlayerCount { get; set; }
    public static int MaxPlayers { get; set; }

    private UdpClient udpClient;
    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        udpClient = new UdpClient { EnableBroadcast = true };
        cancellationTokenSource = new CancellationTokenSource();

        if(GlobalVariableHandler.Instance.ServerName != string.Empty)
            GameName = GlobalVariableHandler.Instance.ServerName;

        Task.Run(() => SendBroadcasts(cancellationTokenSource.Token), cancellationTokenSource.Token);
    }

    private async Task SendBroadcasts(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string message = $"{GameName}|{PlayerCount}|{MaxPlayers}|{GetLocalIPAddress()}|{GlobalVariableHandler.Instance.GamePort}";
                byte[] data = Encoding.UTF8.GetBytes(message);
                foreach (var broadcastAddress in NetworkUtilities.GetBroadcastAddresses())
                {
                    IPEndPoint endpoint = new IPEndPoint(broadcastAddress, BroadcastPort);
                    udpClient.Send(data, data.Length, endpoint);
                }

                await Task.Delay(5000, cancellationToken); // Wait for 5 second or until cancellation
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Broadcasting task canceled.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Broadcasting error: {ex.Message}");
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
        StopBroadcasting();
    }

    private void OnApplicationQuit()
    {
        StopBroadcasting();
    }

    private void StopBroadcasting()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        Debug.Log("ServerBroadcaster stopped.");
    }
}
