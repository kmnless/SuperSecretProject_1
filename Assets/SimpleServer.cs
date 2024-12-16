using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEditor.VersionControl;
using System.Linq;

public class SimpleServer : MonoBehaviour
{
    //[SerializeField] private TMP_InputField InputPort;
    //[SerializeField] private TextMeshProUGUI PlayerNames;

    //private bool IsStarted = false;
    //private bool GameIsOn = false;
    //private int id = 0;

    //private NetworkDriver m_Driver;
    //private NativeList<NetworkConnection> m_Connections;

    //private List<PlayerProperty> players = new List<PlayerProperty>();

    //Deprecated

    //void Start()
    //{
    //    ObjectManager objectManager = ObjectManager.Instance;

    //    objectManager.setServer(gameObject);
    //}

    //public void CreateServer()
    //{
    //    if (IsStarted) { return; }
    //    m_Driver = NetworkDriver.Create();
    //    m_Connections = new NativeList<NetworkConnection>(GlobalVariableHandler.playerCount, Allocator.Persistent);
    //    GlobalVariableHandler.players = new PlayerProperty[GlobalVariableHandler.playerCount];
    //    var endpoint = NetworkEndpoint.AnyIpv4.WithPort(Convert.ToUInt16(InputPort.text));
    //    if (m_Driver.Bind(endpoint) != 0)
    //    {
    //        Debug.LogError($"Failed to bind to port {Convert.ToUInt16(InputPort.text)}.");
    //        return;
    //    }
    //    m_Driver.Listen();
    //    Debug.Log($"Server started at {m_Driver.GetLocalEndpoint()}");
    //    IsStarted = true;
    //}

    //void Update()
    //{
    //    if (!IsStarted) { return; }

    //    m_Driver.ScheduleUpdate().Complete();

    //    // Clean up connections.
    //    for (int i = 0; i < m_Connections.Length; i++)
    //    {
    //        if (!m_Connections[i].IsCreated)
    //        {
    //            m_Connections.RemoveAtSwapBack(i);
    //            i--;
    //        }
    //    }

    //    // Accept new connections.
    //    NetworkConnection c;
    //    while ((c = m_Driver.Accept()) != default)
    //    {
    //        if (GameIsOn)
    //            c.Close(m_Driver);
    //        m_Connections.Add(c);
    //        Debug.Log("Accepted a connection.");
    //    }

    //    for (int i = 0; i < m_Connections.Length; i++)
    //    {
    //        DataStreamReader stream;
    //        NetworkEvent.Type cmd;
    //        while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
    //        {
    //            if (cmd == NetworkEvent.Type.Data)
    //            {
    //                NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
    //                stream.ReadBytes(bytes);

    //                JsonTextReader reader = new JsonTextReader(new StringReader(Encoding.UTF8.GetString(bytes)));
    //                JsonSerializer serializer = new JsonSerializer();
    //                IPacket packet = serializer.Deserialize<IPacket>(reader);

    //                if (packet == null)
    //                {
    //                    Debug.LogError($"GOT EMPTY MESSAGE! i = {i}");
    //                }
    //                else
    //                {
    //                    if (packet.PacketType == "InitPacket")
    //                    {
    //                        JsonTextReader reader2 = new JsonTextReader(new StringReader(Encoding.UTF8.GetString(bytes)));
    //                        InitPacket concretePacket = serializer.Deserialize<InitPacket>(reader2);
    //                        Debug.Log($"Got Init Packet: {concretePacket.name}");

    //                        players.Add(new PlayerProperty(concretePacket.name, id));

    //                        //Debug.Log($"FIRST i: {i} player.l {players.Count}");
    //                        //foreach (var player in players)
    //                        //{
    //                        //    Debug.Log($"name[]: {player.Name}");
    //                        //}

    //                        UpdatePlayerNameText();

    //                        NativeArray<byte> msg = new NativeArray<byte>(Encoding.UTF8.GetBytes(id.ToString()), Allocator.Temp);
    //                        id++;

    //                        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var whisper);
    //                        whisper.WriteBytes(msg);
    //                        m_Driver.EndSend(whisper);

    //                        msg.Dispose();  
    //                    }
    //                    else if (packet.PacketType == "DefaultPacket")
    //                    {
    //                        throw new NotImplementedException("DefaultPacket");
    //                    }
    //                    else if (packet.PacketType == "SpecialPacket")
    //                    {
    //                        throw new NotImplementedException("SpecialPacket");
    //                    }
    //                    else
    //                    {
    //                        throw new ArgumentException("Not standardized message");
    //                    }
    //                }

    //                bytes.Dispose();
    //            }
    //            else if (cmd == NetworkEvent.Type.Disconnect)
    //            {
    //                Debug.Log($"SECOND i: {i} player.l {players.Count} m_con {m_Connections.Length}");
    //                foreach (var player in players)
    //                {
    //                    if (player.Id == i)
    //                    {
    //                        Debug.Log($"{player.Name} LIKVIDIROVAN (ego id {player.Id})");
    //                        players.Remove(player);
    //                        UpdatePlayerNameText();
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //private NativeArray<byte> MakeServerPacket(List<PlayerProperty> playersSyncData, List<SpecialAction> specialActions)
    //{
    //    ServerPacket packet = new ServerPacket(playersSyncData, specialActions);
    //    NativeArray<byte> bytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), Allocator.Persistent);
    //    return bytes;
    //}

    //private void UpdatePlayerNameText()
    //{
    //    if (GameIsOn)
    //        return;

    //    PlayerNames.text = String.Empty; 

    //    foreach (var player in players)
    //    {
    //        PlayerNames.text += player.Name + Environment.NewLine;
    //    }
    //}

    //void OnDestroy()
    //{
    //    if (m_Driver.IsCreated)
    //    {
    //        m_Driver.Dispose();
    //        m_Connections.Dispose();
    //    }
    //}
}
