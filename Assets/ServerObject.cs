
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


public class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 0);
    List<ClientObject> clients = new List<ClientObject>();
    private int connectionCount;
    public Action<string> onConnection;
    public IPEndPoint localEndPoint { get; internal set; }

    public ServerObject(int connectionCount, IPEndPoint endPoint = null)
    {
        if (endPoint != null)
            this.tcpListener = new TcpListener(endPoint);
        this.connectionCount = connectionCount;
    }

    protected internal void RemoveConnection(string id)
    {
        // получаем по id закрытое подключение
        ClientObject client = clients.FirstOrDefault(c => c.Id == id);
        // удаляем его из списка подключений
        if (client != null) clients.Remove(client);
        client?.Close();
    }

    protected internal async Task ListenAsync()
    {
        try
        {
            tcpListener.Start(connectionCount);
            Debug.Log($"Server started on {tcpListener.LocalEndpoint}. Waiting clients to connect...");

            while (true)
            {
                if (clients.Count != connectionCount)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);

                    await Task.Run(clientObject.ProcessAsync);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            Disconnect();
        }
    }

    protected internal async Task BroadcastMessageAsync(string message, string id)
    {
        foreach (var client in clients)
        {
            if (client.Id != id) // if client id != sender id
            {
                Debug.Log(message);
                await client.Writer.WriteLineAsync(message); // data sending
                await client.Writer.FlushAsync();
            }
        }
    }

    // disconnect everyone and shut server down
    protected internal void Disconnect()
    {
        foreach (var client in clients)
        {
            client.Close();
        }
        tcpListener.Stop();
    }
}
