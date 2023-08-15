using System.Net;
using System.Net.Sockets;
using System.Text;

// UTF-8 encoding

namespace Network
{

    class Server
    {
        private readonly int maxConnections;

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

        public async Task startServer()
        {
            // setting up server socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(hostIP), hostPort);
            List<Socket> remoteSockets = new();
            List<Task> proceedTasks = new();
            try
            {
                serverSocket.Bind(endPoint);
                serverSocket.Listen(maxConnections);
                Console.WriteLine($"Server started at {endPoint}. Waiting for connection from users..");
                // Server started

                // TODO) processing clients properly

                //int connectionsCount = 0;
                //while (connectionsCount!=maxConnections)
                //{
                //    // waiting for connection
                //    Socket remoteSocket = await serverSocket.AcceptAsync();
                //    remoteSockets.Add(remoteSocket);
                //    Console.WriteLine($"{remoteSocket.LocalEndPoint} connected");
                //    // adding current client to task list
                //    proceedTasks.Add(proceed(remoteSocket));
                //    connectionsCount++;
                //}
                //while(true)
                //{
                //    // processing connected clients forever
                //    foreach (Task task in proceedTasks)
                //    {
                //        task.Start();
                //    }
                //}    

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private async Task proceed(Socket socket)
        {
            // Processing new client
            string response = Task.Run(async () => await getClientResponseAsync(socket)).Result;
            Console.WriteLine($"Client sent you \"{response}\"");
            // Logic with response....

            // Server reply
            Console.WriteLine("Your response");
            string reply = Console.ReadLine();
            await socket.SendAsync(Encoding.UTF8.GetBytes(reply));
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