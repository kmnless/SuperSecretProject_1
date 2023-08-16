using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network
{

    class Server
    {
        private readonly int maxConnections;
        private object locker = new object();
        private Random r = new Random();

        public int hostPort { get; }

        private readonly string hostIP;

        public Server(string hostIP = "127.0.0.1", int maxConnections = 10)
        {
            this.hostIP = hostIP;
            //this.hostPort = getFreePort();
            hostPort = 59469;
            this.maxConnections = maxConnections;
        }

        private int getFreePort()
        {
            const int minPort = 49152;
            const int maxPort = 65535;
            int port = 0;
            do
            { port = minPort + r.Next(maxPort - minPort); }
            while (System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(l => l.Port == port));

            return port;
        }

        public void startServer()
        {
            // setting up server socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(hostIP), hostPort);

            List<Clients> clients = new();
            try
            {
                serverSocket.Bind(endPoint);
                serverSocket.Listen(maxConnections);
                Console.WriteLine($"Server started at {endPoint}. Waiting for {maxConnections} connections from users..");
                // Server started

                
                int connectionsCount = 0;
                for (int i = 0; i < maxConnections; ++i)
                {
                    new Thread(async () =>
                    {

                        // waiting for connection
                        Socket remoteSocket = await serverSocket.AcceptAsync();
                        clients.Add(new Clients(remoteSocket, connectionsCount));

                        Console.WriteLine($"{clients[connectionsCount].socket.RemoteEndPoint} with id {clients[connectionsCount].id}connected");

                        connectionsCount++;
                    }).Start();
                }
                Console.ReadLine();                                               // KOSTIL))
                Console.WriteLine($"{clients.Count} connected");
                if (connectionsCount == maxConnections)
                {
                    foreach (Clients client in clients)
                    {
                        new Thread(async () =>
                        {
                            while (true)
                            {
                                // Processing new client
                                string response = Task.Run(async () => await getClientResponseAsync(client.socket)).Result;
                                Console.WriteLine($"Client(id = {client.id} sent you \"{response}\"");
                                // Logic with response....

                                // Server reply
                                string reply = "OK))";
                                await client.socket.SendAsync(Encoding.UTF8.GetBytes(reply));
                            }
                        }).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private async Task<string> getClientResponseAsync(Socket remoteSocket)
        {
            // buffer for incoming data
            ArraySegment<byte> buffer = new byte[1024];
            int byteCount = 0;
            string data = string.Empty;

            do
            {
                byteCount = await remoteSocket.ReceiveAsync(buffer, SocketFlags.None);
                data += Encoding.ASCII.GetString(buffer.ToArray(), 0, byteCount);

            } while (remoteSocket.Available > 0);

            return data;
        }
    }

}