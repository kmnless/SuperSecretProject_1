using UnityEngine;
using Unity.Networking.Transport;
using TMPro;
using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections;
using System.Text;
using UnityEngine.XR;
using Unity.VisualScripting;

public class ClientBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputIp;
    [SerializeField] private TMP_InputField InputName;
    [SerializeField] private TMP_InputField InputPort;

    //Deprecated

    //public bool isConnected {get; private set;} = false; 
    //private bool gotId=false;
    //NetworkDriver m_Driver;
    //NetworkConnection m_Connection;
    //void Start()
    //{
    //    // Получите ссылку на экземпляр ObjectManager
    //    ObjectManager objectManager = ObjectManager.Instance;

    //    // Установите ваш объект
    //    objectManager.setClient(gameObject);
    //}
    //public void ConnectToServer()
    //{
    //    m_Driver = NetworkDriver.Create();

    //    var endpoint = NetworkEndpoint.Parse(InputIp.text, Convert.ToUInt16(InputPort.text));
    //    m_Connection = m_Driver.Connect(endpoint);

    //    isConnected = true;

    //}

    //public void ConnectToMyself()
    //{
    //    m_Driver = NetworkDriver.Create();
    //    var endpoint = NetworkEndpoint.Parse(IPAddress.Loopback.ToString(), Convert.ToUInt16(InputPort.text));
    //    m_Connection = m_Driver.Connect(endpoint);
    //    Debug.Log($"Connected to {m_Driver.GetLocalEndpoint()},   {Convert.ToUInt16(InputPort.text)}");

    //    isConnected = true;
    //}

    //private NativeArray<byte> MakeInitPacket(string name)
    //{
    //    InitPacket packet = new InitPacket(name);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
    //    return bytes;
    //}
    //private NativeArray<byte> MakeDefaultPacket(PlayerProperty playerProperty)
    //{
    //    DefaultPacket packet = new DefaultPacket(playerProperty);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
    //    return bytes;
    //}
    //private NativeArray<byte> MakeSpecialPacket(PlayerProperty playerProperty, SpecialAction action)
    //{
    //    SpecialPacket packet = new SpecialPacket(playerProperty, action);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
    //    return bytes;
    //}
    //private NativeArray<byte> MakeMovePacket(byte id, int x, int y)
    //{
    //    MovePacket packet = new MovePacket(id, x, y);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
    //    return bytes;
    //}


    //public void SendMovementSignal(int x,int y) 
    //{

    //}
    //public void SendCaptureSignal(int id) 
    //{

    //}

    //void Update()
    //{
    //    if(!isConnected) { return; }
    //    m_Driver.ScheduleUpdate().Complete();

    //    if (!m_Connection.IsCreated)
    //    {
    //        return;
    //    }
    //    Unity.Collections.DataStreamReader stream;
    //    NetworkEvent.Type cmd;
    //    while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
    //    {
    //        if (cmd == NetworkEvent.Type.Connect)
    //        {
    //            Debug.Log("We are now connected to the server.");
    //            NativeArray<byte> message = MakeInitPacket(InputName.text);
    //            string JsonRead = Encoding.UTF8.GetString(message);
    //            Debug.Log("Sent message: " + JsonRead);
    //            m_Driver.BeginSend(m_Connection, out var writer);
    //            writer.WriteBytes(message);
    //            m_Driver.EndSend(writer);
    //        }
    //        else if (cmd == NetworkEvent.Type.Data)
    //        {
    //            NativeArray<byte> message = new NativeArray<byte>(stream.Length, Allocator.Temp);
    //            stream.ReadBytes(message);
    //            Debug.Log($"Got the message from the server.");
    //            if (!gotId)
    //            {
    //                GlobalVariableHandler.myIndex = Convert.ToInt32(Encoding.UTF8.GetString(message));
    //                Debug.Log("My id is :" + GlobalVariableHandler.myIndex);
    //                gotId = true;
    //            }
    //            else
    //            {
    //                string JsonRead = Encoding.UTF8.GetString(message);
    //                ServerPacket serverPacket = JsonConvert.DeserializeObject<ServerPacket>(JsonRead);
    //                // process message....
    //            }

    //            message.Dispose();
    //        }
    //        else if (cmd == NetworkEvent.Type.Disconnect)
    //        {
    //            Debug.Log("Client got disconnected from server.");
    //            m_Connection = default;
    //        }
    //    }
    //}
    //public void Disconnect()
    //{
    //    if (isConnected)
    //    {
    //        // Отключаем соединение
    //        m_Connection.Disconnect(m_Driver);
    //        m_Connection = default;

    //        // Освобождаем сетевой драйвер
    //        m_Driver.Dispose();

    //        // Устанавливаем флаг соединения в false
    //        isConnected = false;

    //        Debug.Log("Disconnected from the server.");
    //    }
    //}

    //void OnDestroy()
    //{
    //    m_Driver.Dispose();
    //}

}