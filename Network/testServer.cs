using System.Net.Sockets;

using System.Net;

string host = "127.0.0.1";
int port = 8888;
IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(host), port);
ServerObject server = new ServerObject(3, endPoint);
await server.ListenAsync();