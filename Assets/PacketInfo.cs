using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Xml.Linq;
using UnityEngine.Experimental.GlobalIllumination;

public enum Action
{
    KILL_BASE,
    TAKE_FLAG,
    LEVEUP,
}

public interface IPacket
{
    string PacketType { get; set; }
    public string ToString();
}

public class MovePacket : IPacket
{
    public string PacketType { get; set; }
    public byte initiatorId;
    public int X;
    public int Y;
    public MovePacket(byte initiatorId, int X, int Y)
    {
        PacketType = "MovePacket";
        this.initiatorId = initiatorId;
        this.X = X;
        this.Y = Y;
    }
    override public string ToString() 
    {
        return $"PacketType: {PacketType}, initiatorId: {initiatorId}, X: {X}, Y: {Y}";
    }
}
public class SpecialAction : IPacket
{
    public string PacketType { get; set; }
    public byte initiatorId;
    public Action action;
    public byte subjectID;
    public SpecialAction(byte initiatorId, Action action, byte subjectId)
    {
        this.initiatorId = initiatorId;
        this.action = action;
        this.subjectID = subjectId;
    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}, initiatorId: {initiatorId}, Action: {action}, subjectID: {subjectID}";
    }
}
public class MapPacket : IPacket
{
    public string PacketType { get; set; }
    public byte[,] mapTerrainTexture;
    public int[,] buildings;
    public MapPacket(byte[,] mapTerrainTexture, int[,] buildings)
    {
        PacketType = "MapPacket";
        this.mapTerrainTexture = mapTerrainTexture;
        this.buildings = buildings;

    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}";
    }
}
public class DefaultPacket : IPacket
{
    public string PacketType { get; set; }
    public PlayerProperty playerProperty;
    public DefaultPacket(PlayerProperty playerProperty)
    {
        PacketType = "DefaultPacket";
        this.playerProperty = playerProperty;
    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}, (no idea if ToString() implemented normally) PlayerProperty: {playerProperty.ToString()}";
    }
}
public class SpecialPacket : IPacket
{
    public string PacketType { get; set; }
    public PlayerProperty playerProperty;
    public SpecialAction action;
    public SpecialPacket(PlayerProperty playerProperty, SpecialAction specialAction)
    {
        this.PacketType = "SpecialPacket";
        this.playerProperty = playerProperty;
        this.action = specialAction;
    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}, (no idea if ToString() implemented normally) PlayerProperty: {playerProperty.ToString()}, SpecialAction: {action}";
    }
}
public class InitPacket : IPacket
{
    public string PacketType { get; set; }
    public string name;
    //public int id;                 // ??????

    public InitPacket(string name)
    {
        PacketType = "InitPacket";
        this.name = name;
    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}, name: {name}";
    }
}


public class ServerPacket : IPacket
{
    public string PacketType { get; set; }
    public List<PlayerProperty> playersSyncData;
    public List<SpecialAction> specialActions;
    public ServerPacket(List<PlayerProperty> playersSyncData, List<SpecialAction> specialActions)
    {
        this.playersSyncData = playersSyncData;
        this.specialActions = specialActions;
    }

    override public string ToString()
    {
        return $"PacketType: {PacketType}";
    }
}
