using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Experimental.GlobalIllumination;

public enum Action
{
    KILL_BASE,
    TAKE_FLAG,
    LEVEUP,
}
public struct SpecialAction
{
    byte initiatorID;
    Action action;
    byte subjectID;
}
public struct MapPacket
{
    byte[,] mapTerrainTexture;
    int[,] buildings;
}
public struct Packet
{
    List<PlayerProperty> playersSyncData;
    List<SpecialAction> specialActions; 
}
