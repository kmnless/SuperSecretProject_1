using System.Net.Sockets;
using System.Net;

public class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 0);
    List<ClientObject> clients = new List<ClientObject>();
    private int connectionCount;
    public ServerObject(int connectionCount, IPEndPoint? endPoint = null)
    {
        if( endPoint != null )
            this.tcpListener = new TcpListener(endPoint);
        this.connectionCount = connectionCount;
    }

    protected internal void RemoveConnection(string id)
    {
        // получаем по id закрытое подключение
        ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
        // удаляем его из списка подключений
        if (client != null) clients.Remove(client);
        client?.Close();
    }

    protected internal async Task ListenAsync()
    {
        try
        {
            tcpListener.Start(connectionCount);
            Console.WriteLine($"Server started on {tcpListener.LocalEndpoint}. Waiting clients to connect...");

            while (true)
            {
                if (clients.Count != connectionCount)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);

                    Task.Run(clientObject.ProcessAsync);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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
