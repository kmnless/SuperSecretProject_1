using System;
using Unity.Collections;
using Unity;
using Unity.Netcode;
using UnityEngine;

public struct PlayerProperty : INetworkSerializable, IEquatable<PlayerProperty>
{
    public int Id;
    public FixedString512Bytes Name;
    public int Strength;                        // Strength to kill players
    public int Level;                           // Level is for items
    public int CurrentXP;                       // Current XP
    public float StrengthMultiplier;            // From items
    public int NeededXP;                        // XP for next level
    public float MultiplierXP;                  // From items (might be deleted)
    public float StrengthMultiplierGain;        // From items
    public float Money;                         // Money == points. Money is target to win. Items purchased by money
    public float MoneyIncome;                   // Money per minute
    public float Diamonds;                      // Diamonds can be uptained from outposts
    public float DiamondsIncome;                // Diamonds per minute
    public float MoveSpeed;                     // Movespeed
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
        MoneyIncome = 50;
        Diamonds = 0;
        DiamondsIncome = 0;
        MoveSpeed = 0.1f;
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
        serializer.SerializeValue(ref MoneyIncome);
        serializer.SerializeValue(ref Diamonds);
        serializer.SerializeValue(ref DiamondsIncome);
        serializer.SerializeValue(ref Color);
        serializer.SerializeValue(ref MoveSpeed);
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
