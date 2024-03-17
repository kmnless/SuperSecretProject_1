using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using UnityEngine.Experimental.GlobalIllumination;

public enum Action
{
    KILL_BASE,
    TAKE_FLAG,
    LEVEUP,
}

public struct MovePacket 
{
    public MovePacket(byte initiatorId, int X,  int Y) 
    {
        this.initiatorId = initiatorId;
        this.X = X; 
        this.Y = Y; 
    }
    public byte initiatorId;
    public int X;
    public int Y;

}
public struct SpecialAction
{
    public SpecialAction(byte initiatorId, Action action, byte subjectId)
    { 
        this.initiatorID = initiatorId;
        this.action = action;
        this.subjectID = subjectId;
    }

    public byte initiatorID;
    public Action action;
    public byte subjectID;
}
public struct MapPacket
{
    public MapPacket(byte[,] mapTerrainTexture, int[,] buildings)
    {
        packetType = "MapPacket";
        this.mapTerrainTexture = mapTerrainTexture;
        this.buildings = buildings;
        
    }
    public string packetType;
    public byte[,] mapTerrainTexture;
    public int[,] buildings;
}
public struct DefaultPacket
{
    public DefaultPacket(PlayerProperty playerProperty)
    {
        this.packetType = "DefaultPacket";
        this.playerProperty = playerProperty;
    }

    public string packetType;
    public PlayerProperty playerProperty;
}
public struct SpecialPacket
{
    public SpecialPacket(PlayerProperty playerProperty, SpecialAction specialAction)
    {
        this.packetType = "SpecialPacket";
        this.playerProperty = playerProperty;
        this.action = specialAction;
    }

    public string packetType;
    public PlayerProperty playerProperty;
    public SpecialAction action;
}
public struct InitPacket
{
    public InitPacket(string name)
    {
        packetType = "InitPacket";
        this.name = name;
    }

    public string packetType;
    public string name;
    //public int id;                 // ??????
}

public struct ServerPacket
{
    public ServerPacket(List<PlayerProperty> playersSyncData, List<SpecialAction> specialActions)
    {
        this.playersSyncData = playersSyncData;
        this.specialActions = specialActions;
    }

    public List<PlayerProperty> playersSyncData;
    public List<SpecialAction> specialActions;
}
