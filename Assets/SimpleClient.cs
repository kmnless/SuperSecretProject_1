using UnityEngine;
using Unity.Networking.Transport;
using TMPro;
using System;

public class ClientBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputIp;
    [SerializeField] private TMP_InputField InputName;
    [SerializeField] private TMP_InputField InputPort;


    bool isConnected = false;

    NetworkDriver m_Driver;
    NetworkConnection m_Connection;

    public void ConnectToServer()
    {
        m_Driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.Parse(InputIp.text, Convert.ToUInt16(InputPort.text));
        m_Connection = m_Driver.Connect(endpoint);

        isConnected = true;

    }

    public void ConnectToMyself()
    {
        m_Driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.Parse(NetworkEndpoint.LoopbackIpv4.ToString(), Convert.ToUInt16(InputPort.text));
        m_Connection = m_Driver.Connect(endpoint);
        Debug.Log($"Connected to {m_Driver.GetLocalEndpoint()},   {Convert.ToUInt16(InputPort.text)}");

        isConnected = true;
    }

    void Update()
    {
        if(!isConnected) { return; Debug.Log("1"); }
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }
        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            Debug.Log("2");

            if (cmd == NetworkEvent.Type.Connect)
            {

                Debug.Log("We are now connected to the server.");

                uint value = 1;
                m_Driver.BeginSend(m_Connection, out var writer);
                writer.WriteUInt(value);
                m_Driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                Debug.Log("3");

                uint value = stream.ReadUInt();
                Debug.Log($"Got the value {value} back from the server.");

                m_Connection.Disconnect(m_Driver);
                m_Connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("4");

                Debug.Log("Client got disconnected from server.");
                m_Connection = default;
            }
        }
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
    }

}