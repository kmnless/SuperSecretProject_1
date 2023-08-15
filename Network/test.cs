using Network;

Server server = new Server("127.0.0.1", 4);
await server.startServer();

//Client client = new Client("127.0.0.1", 59469);
//await client.startClient();

Console.ReadLine();