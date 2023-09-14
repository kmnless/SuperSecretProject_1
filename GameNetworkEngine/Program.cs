const string host = "127.0.0.1";
const int port = 8080;

IServerHandler handler = new TextServerHandler()
{
    EncodingType = EncodingType.UTF8
};
Server server = new Server(host, port, handler);