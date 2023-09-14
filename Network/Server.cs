using NetworkEngine.Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEngine
{
    public class Server
    {
        public event Action<IPEndPoint>? ServerStarted;
        public event Action<Socket>? Connected;             // TODO: <ConnectionInfo> type against <Socket> ?
        public event Action<string>? MessageReceived;

        public IPAddress Host { get; private set; }
        public int Port { get; private set; }

        private IPEndPoint serverEndPoint;
        private Socket serverSocket;
        private Socket? remoteSocket;

        private int connectionCount;

        private IServerHandler handler;

        public Server(string host, int port, IServerHandler handler, int connectionCount)
        {
            Host = IPAddress.Parse(host);
            Port = port;
            this.handler = handler;
            handler.DataDecoded += Handler_DataDecoded;

            this.connectionCount = connectionCount;

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverEndPoint = new IPEndPoint(Host, Port);
        }

        private void Handler_DataDecoded(string message)
        {
            MessageReceived?.Invoke(message);
        }

        public async Task StartAsync()
        {
            serverSocket.Bind(serverEndPoint);
            serverSocket.Listen(connectionCount);
            ServerStarted?.Invoke(serverEndPoint);

            remoteSocket = await serverSocket.AcceptAsync();
            Connected?.Invoke(remoteSocket);

            await handler.HandleAsync(remoteSocket);
        }

        public async Task SendAsync(string message)
        {
            await handler.SendTo(remoteSocket, message);
        }
    }
}
