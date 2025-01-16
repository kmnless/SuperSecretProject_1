using System;
using Unity.Collections;
using Unity;
using Unity.Netcode;
using UnityEngine;

public struct PlayerProperty : INetworkSerializable, IEquatable<PlayerProperty>
{
    public int Id;
    public FixedString512Bytes Name;
    public int Strength;
    public int Level;
    public int CurrentXP;
    public float StrengthMultiplier;
    public int NeededXP;
    public float MultiplierXP;
    public float StrengthMultiplierGain;
    public int Money;
    public Vector4 Color; // Vector4 instead of Color

    public PlayerProperty(string name, int id)
    {
        Id = id;
        Name = name;
        Strength = 10;
        Level = 1;
        CurrentXP = 0;
        StrengthMultiplier = 1f;
        NeededXP = 50;
        MultiplierXP = 1.5f;
        Money = 50;
        //Color = color;
        Color = new Vector4(0, 0, 0, 1); // black
        StrengthMultiplierGain = 1.15f;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Strength);
        serializer.SerializeValue(ref Level);
        serializer.SerializeValue(ref CurrentXP);
        serializer.SerializeValue(ref StrengthMultiplier);
        serializer.SerializeValue(ref NeededXP);
        serializer.SerializeValue(ref MultiplierXP);
        serializer.SerializeValue(ref StrengthMultiplierGain);
        serializer.SerializeValue(ref Money);
        serializer.SerializeValue(ref Color);
    }
    public bool Equals(PlayerProperty other)
    {
        return Id == other.Id &&
               Name.Equals(other.Name) &&
               Strength == other.Strength &&
               Level == other.Level &&
               CurrentXP == other.CurrentXP &&
               StrengthMultiplier.Equals(other.StrengthMultiplier) &&
               NeededXP == other.NeededXP &&
               MultiplierXP.Equals(other.MultiplierXP) &&
               StrengthMultiplierGain.Equals(other.StrengthMultiplierGain) &&
               Money == other.Money &&
               Equals(Color, other.Color);
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerProperty other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}
