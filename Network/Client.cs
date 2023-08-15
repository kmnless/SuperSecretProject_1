using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

// UTF-8 encoding
namespace Network
{
    class Client
    {
        private int serverPort;
        private readonly string serverIP;
        private Random r = new Random();

        public Client(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
        }

        public async Task startClient()
        {
            // creating a socket
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // connect and send
                await clientSocket.ConnectAsync(serverIP, serverPort);

                while (true)
                {
                    Console.WriteLine("Input a message");
                    string message = getMessage();


                    await clientSocket.SendAsync(Encoding.UTF8.GetBytes(message));

                    string response = getServerResponseAsync(clientSocket).Result;

                    // Logic....
                    Console.WriteLine($"Server response is {response}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string getMessage()
        {
            string? message = Console.ReadLine();
            return message == null ? "empty" : message;
        }

        private async Task<string> getServerResponseAsync(Socket socket)
        {
            // buffer for incoming data
            ArraySegment<byte> buffer = new byte[1024];
            int byteCount = 0;
            string data = string.Empty;

            do
            {
                byteCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
                data += Encoding.ASCII.GetString(buffer.ToArray(), 0, byteCount);

            } while (socket.Available > 0);

            return data;
        }
    }

}