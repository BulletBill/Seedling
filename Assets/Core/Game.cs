using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Game : Node
{
    [Export] public PackedScene DamageNumberPrefab;
    public static Game Singleton { get; protected set; }
    static RandomNumberGenerator RNG = new();

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public static int GetIntInRange(int Min, int Max)
    {
        return Game.RNG.RandiRange(Min, Max);
    }

    public static float GetFloatInRange(float Min, float Max)
    {
        return Game.RNG.RandfRange(Min, Max);
    }

    public static String GetTimeFromSeconds(float InTime)
    {
        int Seconds = Mathf.FloorToInt(InTime % 60.0f);
        int Minutes = Mathf.FloorToInt((InTime/60.0f) % 60.0f);
        int Hours = Mathf.FloorToInt((InTime/60.0f) / 60.0f);

        String ret = "";
        if (Hours > 0) { ret += Hours.ToString() + ":"; }
        if (Minutes > 0) { ret += Minutes.ToString() + ":"; } else { ret += "0:"; }
        if (Seconds < 10) { ret += "0"; }
        ret += Seconds.ToString();

        return ret;
    }

    public static void SpawnResourceNumber(Vector2 Location, int Amount, ECurrencyType Type)
    {
        if (Game.Singleton == null) return;
        if (Game.Singleton.DamageNumberPrefab == null) return;

        DamageNumber NewNumber = Game.Singleton.DamageNumberPrefab.InstantiateOrNull<DamageNumber>();
        if (NewNumber != null)
        {
            MainMap.Singleton.AddChild(NewNumber);
            NewNumber.AssignResource(Location, Amount, Type);
        }
    }

    static readonly List<Vector2> ClusterOffsets = new() {
        new Vector2(-15,-15),
        new Vector2(15,-15),
        new Vector2(-15,15),
        new Vector2(15,15)
    };
    public static void SpawnResourceCluster(Vector2 Center, int Substance, int Flow, int Breath, int Energy)
    {
        if (Game.Singleton == null) return;
        if (Game.Singleton.DamageNumberPrefab == null) return;

        int index = 0;
        if (Substance != 0)
        {
            SpawnResourceNumber(Center + ClusterOffsets[index++], Substance, ECurrencyType.Substance);
        }
        if (Flow != 0)
        {
            SpawnResourceNumber(Center + ClusterOffsets[index++], Flow, ECurrencyType.Flow);
        }
        if (Breath != 0)
        {
            SpawnResourceNumber(Center + ClusterOffsets[index++], Breath, ECurrencyType.Breath);
        }
        if (Energy != 0)
        {
            SpawnResourceNumber(Center + ClusterOffsets[index++], Energy, ECurrencyType.Energy);
        }
    }
}
