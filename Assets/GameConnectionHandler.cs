using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameConnectionHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputIp;
    [SerializeField] private TMP_InputField InputName;
    [SerializeField] private TMP_InputField InputPort;
    [SerializeField] private Button HostButton;
    [SerializeField] private TMP_Text Players;

    private bool IsHosted = false;

    private ServerObject server;
    public async void StartServerAsync(int maxPlayers)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(InputIp.text), Convert.ToInt32(InputPort.text));
        server = new ServerObject(maxPlayers, endPoint);
        IsHosted = true;
        InputIp.enabled = false; 
        InputPort.enabled = false;
        HostButton.interactable = false;

        GlobalVariableHandler.serverIPEndPoint = endPoint;
        await server.ListenAsync();
    }

    public async void ConnectToServerAsync()
    {
        string host = InputIp.text;
        int port = Convert.ToInt32(InputPort.text);
        string name = InputName.text;
        using TcpClient client = new TcpClient();
        StreamReader? Reader = null;
        StreamWriter? Writer = null;
        try
        {
            client.Connect(host, port); //подключение клиента
            Debug.Log($"{name} has connected to {host}:{port}");
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            if (Writer is null || Reader is null) return;
            // запускаем новый поток для получения данных
            Task.Run(() => ReceiveMessageAsync(Reader));
            // запускаем ввод сообщений
            await SendMessageAsync(Writer);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        Writer?.Close();
        Reader?.Close();
    }

    //public async void ConnectToServerAsync(string name)
    //{
    //    using TcpClient client = new TcpClient();
    //    StreamReader? Reader = null;
    //    StreamWriter? Writer = null;
    //    IPEndPoint endPoint = GlobalVariableHandler.serverIPEndPoint;
    //    try
    //    {
    //        client.Connect(endPoint); //подключение клиента
    //        Debug.Log($"{name} has connected to {endPoint}");
    //        Reader = new StreamReader(client.GetStream());
    //        Writer = new StreamWriter(client.GetStream());
    //        if (Writer is null || Reader is null) return;
    //        // запускаем новый поток для получения данных
    //        await Task.Run(() => ReceiveMessageAsync(Reader));
    //        // запускаем ввод сообщений
    //        await SendMessageAsync(Writer);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogException(ex);
    //    }
    //    Writer?.Close();
    //    Reader?.Close();
    //}

    public async void ConnectToLocalServerAsync()
    {
        if (IsHosted)
        {
            using TcpClient client = new TcpClient();
            StreamReader? Reader = null;
            StreamWriter? Writer = null;
            try
            {
                client.Connect(IPAddress.Parse(InputIp.text), Convert.ToInt32(InputPort.text)); //подключение клиента
                Debug.Log($"local client({InputName.text}) connected to localhost");
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                await SendMessageAsync(Writer);
                if (Writer is null || Reader is null) return;
                // запускаем новый поток для получения данных
                Task.Run(() => ReceiveMessageAsync(Reader));
                // запускаем ввод сообщений
                await SendMessageAsync(Writer);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            Writer?.Close();
            Reader?.Close();
        }
    }

    // отправка сообщений
    private async Task SendMessageAsync(StreamWriter writer)
    {
        // сначала отправляем имя
        await writer.WriteLineAsync(InputName.text);
        await writer.FlushAsync();
        //Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");                // ---------------------------------- ?

        while (true)
        {
            string? message = Console.ReadLine();                                                           // ---------------------- ?
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }
    }
    // получение сообщений
    private async Task ReceiveMessageAsync(StreamReader reader)
    {
        while (true)
        {
            try
            {
                // считываем ответ в виде строки
                string? message = await reader.ReadLineAsync();
                // если пустой ответ, ничего не выводим на консоль
                if (string.IsNullOrEmpty(message)) continue;
                //Print(message);//вывод сообщения        
                //Debug.Log(message);
            }
            catch
            {
                break;
            }
        }
    }
    // чтобы полученное сообщение не накладывалось на ввод нового сообщения
    //void Print(string message)
    //{
    //    if (System.OperatingSystem.IsWindows())    // если ОС Windows
    //    {
    //        var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
    //        int left = position.Left;   // смещение в символах относительно левого края
    //        int top = position.Top;     // смещение в строках относительно верха
    //                                    // копируем ранее введенные символы в строке на следующую строку
    //        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
    //        // устанавливаем курсор в начало текущей строки
    //        Console.SetCursorPosition(0, top);
    //        // в текущей строке выводит полученное сообщение
    //        Console.WriteLine(message);
    //        // переносим курсор на следующую строку
    //        // и пользователь продолжает ввод уже на следующей строке
    //        Console.SetCursorPosition(left, top + 1);
    //    }
    //    else Console.WriteLine(message);
    //}
}

