using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

public class SimpleServer : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputPort;

    private bool IsStarted = false;
    private int id=0;

    NetworkDriver m_Driver;
    NativeList<NetworkConnection> m_Connections;

    List<PlayerProperty> players = new List<PlayerProperty>();

    void Start()
    {
        ObjectManager objectManager = ObjectManager.Instance;

        // Установите ваш объект
        objectManager.setServer(gameObject);
    }
    public void CreateServer()
    {

        //Test();
        if (IsStarted) { return; }
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(GlobalVariableHandler.playerCount, Allocator.Persistent);
        GlobalVariableHandler.players=new PlayerProperty[GlobalVariableHandler.playerCount];
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(Convert.ToUInt16(InputPort.text));
        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.LogError($"Failed to bind to port {Convert.ToUInt16(InputPort.text)}.");
            return;
        }
        m_Driver.Listen();
        Debug.Log($"Server started at {m_Driver.GetLocalEndpoint()}");
        IsStarted = true;
    }

    void Test()
    {
        InitPacket packet = new InitPacket("name1213");
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)),Allocator.Persistent);
        Debug.Log($"Serialize = {JsonConvert.SerializeObject(packet)}");
        string JsonRead = Encoding.UTF8.GetString(bytes);
        Debug.Log($"JsonRead = {JsonRead}");
        if (JsonRead.Contains("InitPacket"))
        {
            InitPacket packet2 = JsonConvert.DeserializeObject<InitPacket>(JsonRead);
            Debug.Log(packet2.name);
        }
    }
    void Update()
    {
        if (!IsStarted) { return; }

        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections.
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        // Accept new connections.
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);
            
            Debug.Log("Accepted a connection.");
        }


        for (int i = 0; i < m_Connections.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    // logic.....
                    NativeArray<byte> bytes = new NativeArray<byte>();
                    stream.ReadBytes(bytes);
                    string JsonRead = Encoding.UTF8.GetString(bytes);
                    if (JsonRead.Contains("InitPacket"))
                    {
                        InitPacket packet = JsonConvert.DeserializeObject<InitPacket>(JsonRead);
                        players[id]=new PlayerProperty(packet.name,GlobalVariableHandler.colors[id]);
                        NativeArray<byte> msg = new NativeArray<byte>(Encoding.UTF8.GetBytes(id.ToString()),Allocator.Persistent);
                        id++;

                        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var whisper); // v teorii mozhet obostatsa [i] mojno kak id luche.
                        whisper.WriteBytes(msg);
                        
                        m_Driver.EndSend(whisper);

                    }
                    else if (JsonRead.Contains("MapPacket"))
                    {
                        MapPacket packet = JsonConvert.DeserializeObject<MapPacket>(JsonRead);




                    }
                    else if (JsonRead.Contains("DefaultPacket"))
                    {
                        DefaultPacket packet = JsonConvert.DeserializeObject<DefaultPacket>(JsonRead);
                        


                    }
                    else if (JsonRead.Contains("SpecialPacket"))
                    {



                        SpecialPacket packet = JsonConvert.DeserializeObject<SpecialPacket>(JsonRead);
                    }
                    else
                    {
                        throw new ArgumentException("Not standardized message");
                    }

                    NativeArray<byte> serverMessage = MakeServerPacket(null,null);

                    m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteBytes(serverMessage);
                    m_Driver.EndSend(writer);
                }
            }
        }
    }

    NativeArray<byte> MakeServerPacket(List<PlayerProperty> playersSyncData, List<SpecialAction> specialActions)
    {
        ServerPacket packet = new ServerPacket(playersSyncData, specialActions);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
        return bytes;
    }

    void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
    }
}
