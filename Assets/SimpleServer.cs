using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class SimpleServer : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputPort;

    private bool IsStarted = false;

    NetworkDriver m_Driver;
    NativeList<NetworkConnection> m_Connections;

    public void CreateServer()
    {
        if(IsStarted) { return; }
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(10, Allocator.Persistent);

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

    void Update()
    {
        if (!IsStarted) { return; }

        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections(убрать вис€чие и неактивные подключени€).
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
                    uint number = stream.ReadUInt();
                    Debug.Log($"Got {number} from a client, adding 2 to it.");
                    number += 2;

                    m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteUInt(number);
                    m_Driver.EndSend(writer);
                }
            }
        }
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
