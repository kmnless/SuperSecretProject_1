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

public class IPacket
{
    public string PacketType { get; set; }
}

public class MovePacket : IPacket
{
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
public class SpecialAction
{
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
        return $"initiatorId: {initiatorId}, Action: {action}, subjectID: {subjectID}";
    }
}
public class MapPacket : IPacket
{
    public int sizeX;
    public int sizeY;
    public int seed;
    public int baseCount;
    public int flagAmount;
    public int middleFlagAmount;
    public int minimalDistance;
    public int safePlaceRadius;
    public double terrainScale;
    public int smoothRange;
    public float smoothCoef;
    public float contrast;
    public float clip;
    public int roadGenerationComplexity;

    public MapPacket(int sizeX, int sizeY, int seed, int baseCount, int flagAmount, int middleFlagAmount, int minimalDistance, int safePlaceRadius,
            double terrainScale, int smoothRange, float smoothCoef, float contrast, float clip, int roadGenerationComplexity = 80)
    {
        PacketType = "MapPacket";

        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.seed = seed;
        this.baseCount = baseCount;
        this.flagAmount = flagAmount;
        this.middleFlagAmount = middleFlagAmount;
        this.minimalDistance = minimalDistance;
        this.safePlaceRadius = safePlaceRadius;
        this.terrainScale = terrainScale;
        this.smoothRange = smoothRange;
        this.smoothCoef = smoothCoef;
        this.contrast = contrast;
        this.clip = clip;
        this.roadGenerationComplexity = roadGenerationComplexity;
    }
    override public string ToString()
    {
        return $"PacketType: {PacketType}, seed: {seed}";
    }
}
public class DefaultPacket : IPacket
{
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
    public string name;
    //public int id;                 // ?????? ne

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
