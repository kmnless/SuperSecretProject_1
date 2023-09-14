using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEngine.Handling
{
    public interface IServerHandler
    {
        public event Action<string>? DataDecoded;
        public EncodingType EncodingType { get; init; }
        public int BufferSize { get; set; }
        public Task HandleAsync(Socket remoteSocket);
        public Task SendTo(Socket remoteSocket, string message);
    }
}
