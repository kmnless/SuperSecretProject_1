using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;


public class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    TcpClient client;
    ServerObject server; // объект сервера

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        // получаем NetworkStream для взаимодействия с сервером
        var stream = client.GetStream();
        // создаем StreamReader для чтения данных
        Reader = new StreamReader(stream);
        // создаем StreamWriter для отправки данных
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        try
        {
            // получаем имя пользователя
            string? userName = await Reader.ReadLineAsync();
            string? message = $"{userName} connected";
            // посылаем сообщение о подключении всем подключенным пользователям
            await server.BroadcastMessageAsync(message, Id);
            Debug.Log(message);
            // в бесконечном цикле получаем сообщения от клиента
            while (true)
            {
                try
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    message = $"{userName}: {message}";
                    //Debug.Log(message);
                    await server.BroadcastMessageAsync(message, Id);
                }
                catch
                {
                    message = $"{userName} disconnected";
                    //Debug.Log(message);
                    await server.BroadcastMessageAsync(message, Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            // в случае выхода из цикла закрываем ресурсы
            server.RemoveConnection(Id);
        }
    }
    // закрытие подключения
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}
