using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEngine.Handling
{
    public class TextServerHandler : IServerHandler
    {
        public event Action<string>? DataDecoded;
        public int BufferSize { get; set; } = 256;
        public required EncodingType EncodingType { get; init; }
        public Encoding CommunicationEncoding { get; private set; } = Encoding.ASCII;
        public async Task HandleAsync(Socket remoteSocket)
        {
            CommunicationEncoding = EncodingType switch
            {
                EncodingType.ASCII => Encoding.ASCII,
                EncodingType.UTF8 => Encoding.UTF8,
                _ => throw new NotSupportedException(),                 // Own exception ?
            };

            int byteCount;
            byte[] buffer;
            string input;

            while(true)
            {
                buffer = new byte[BufferSize];
                byteCount = 0;
                input = string.Empty;

                do
                {
                    byteCount = await remoteSocket.ReceiveAsync(buffer);
                    input += CommunicationEncoding.GetString(buffer, 0, byteCount);
                } while (remoteSocket.Available > 0);

                DataDecoded?.Invoke(input);
            }
        }

        public async Task SendTo(Socket remoteSocket, string message)
        {
            await remoteSocket.SendAsync(CommunicationEncoding.GetBytes(message));
        }
    }
}
