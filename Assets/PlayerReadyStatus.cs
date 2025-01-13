using System;
using System.Xml.Linq;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerReadyStatus : INetworkSerializable, IEquatable<PlayerReadyStatus>
{
    public int Id;
    public FixedString512Bytes Nickname;
    public bool IsReady;

    public PlayerReadyStatus(int id, FixedString512Bytes Nickname)
    {  
        Id = id;
        this.Nickname = Nickname;
        IsReady = false;
    }

    public bool Equals(PlayerReadyStatus other)
    {
        return Nickname.Equals(other.Nickname) && IsReady == other.IsReady && Id == other.Id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Nickname);
        serializer.SerializeValue(ref IsReady);
    }
    public override bool Equals(object obj)
    {
        return obj is PlayerProperty other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Nickname);
    }
}