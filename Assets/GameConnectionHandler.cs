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
            client.Connect(host, port); //����������� �������
            Debug.Log($"{name} has connected to {host}:{port}");
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            if (Writer is null || Reader is null) return;
            // ��������� ����� ����� ��� ��������� ������
            Task.Run(() => ReceiveMessageAsync(Reader));
            // ��������� ���� ���������
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
    //        client.Connect(endPoint); //����������� �������
    //        Debug.Log($"{name} has connected to {endPoint}");
    //        Reader = new StreamReader(client.GetStream());
    //        Writer = new StreamWriter(client.GetStream());
    //        if (Writer is null || Reader is null) return;
    //        // ��������� ����� ����� ��� ��������� ������
    //        await Task.Run(() => ReceiveMessageAsync(Reader));
    //        // ��������� ���� ���������
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
                client.Connect(IPAddress.Parse(InputIp.text), Convert.ToInt32(InputPort.text)); //����������� �������
                Debug.Log($"local client({InputName.text}) connected to localhost");
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                await SendMessageAsync(Writer);
                if (Writer is null || Reader is null) return;
                // ��������� ����� ����� ��� ��������� ������
                Task.Run(() => ReceiveMessageAsync(Reader));
                // ��������� ���� ���������
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

    // �������� ���������
    private async Task SendMessageAsync(StreamWriter writer)
    {
        // ������� ���������� ���
        await writer.WriteLineAsync(InputName.text);
        await writer.FlushAsync();
        //Console.WriteLine("��� �������� ��������� ������� ��������� � ������� Enter");                // ---------------------------------- ?

        while (true)
        {
            string? message = Console.ReadLine();                                                           // ---------------------- ?
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }
    }
    // ��������� ���������
    private async Task ReceiveMessageAsync(StreamReader reader)
    {
        while (true)
        {
            try
            {
                // ��������� ����� � ���� ������
                string? message = await reader.ReadLineAsync();
                // ���� ������ �����, ������ �� ������� �� �������
                if (string.IsNullOrEmpty(message)) continue;
                //Print(message);//����� ���������        
                //Debug.Log(message);
            }
            catch
            {
                break;
            }
        }
    }
    // ����� ���������� ��������� �� ������������� �� ���� ������ ���������
    //void Print(string message)
    //{
    //    if (System.OperatingSystem.IsWindows())    // ���� �� Windows
    //    {
    //        var position = Console.GetCursorPosition(); // �������� ������� ������� �������
    //        int left = position.Left;   // �������� � �������� ������������ ������ ����
    //        int top = position.Top;     // �������� � ������� ������������ �����
    //                                    // �������� ����� ��������� ������� � ������ �� ��������� ������
    //        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
    //        // ������������� ������ � ������ ������� ������
    //        Console.SetCursorPosition(0, top);
    //        // � ������� ������ ������� ���������� ���������
    //        Console.WriteLine(message);
    //        // ��������� ������ �� ��������� ������
    //        // � ������������ ���������� ���� ��� �� ��������� ������
    //        Console.SetCursorPosition(left, top + 1);
    //    }
    //    else Console.WriteLine(message);
    //}
}
