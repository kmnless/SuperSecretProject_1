using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkEngine;
using NetworkEngine.Handling;

const string host = "127.0.0.1";
const int port = 8080;

IServerHandler handler = new TextServerHandler()
{
    EncodingType = EncodingType.UTF8
};
Server server = new Server(host, port, handler, 2);
await server.StartAsync();