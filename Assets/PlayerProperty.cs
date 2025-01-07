using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data.Common;
using UnityEngine;
 public struct PlayerProperty
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Level { get; set; }
    public int CurrentXP { get; set; }
    public float StrengthMultiplier { get; set; }
    public int NeededXP { get; set; }
    public float MultiplierXP { get; set; }
    public float StrengthMultiplierGain{get;set;}
    public int Money { get; set; }
    public Color Color  { get; set; }
    public PlayerProperty(string name, int id)
    {
        Id = id;
        Strength=10;
        Level=1;
        CurrentXP=0;
        StrengthMultiplier=1f;
        NeededXP=50;
        MultiplierXP=1.5f;
        Money=50;
        Color=Color.black;
        StrengthMultiplierGain=1.15f;
        Name=name;
    }
}