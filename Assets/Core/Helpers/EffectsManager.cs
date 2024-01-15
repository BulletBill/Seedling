using Godot;
using System;
using System.Collections.Generic;

public partial class EffectsManager : Node
{
    public static EffectsManager Singleton { get; protected set; }
    PackedScene DamageNumberPrefab;

    public EffectsManager()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        DamageNumberPrefab = ResourceLoader.Load<PackedScene>("res://Assets/Player/Resources/DamageNumber.tscn");
    }

    public static void SpawnResourceNumber(Vector2 Location, int Amount, ECurrencyType Type)
    {
        if (Singleton == null) return;
        if (Singleton.DamageNumberPrefab == null) return;

        DamageNumber NewNumber = Singleton.DamageNumberPrefab.InstantiateOrNull<DamageNumber>();
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
        if (Singleton == null) return;
        if (Singleton.DamageNumberPrefab == null) return;

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
