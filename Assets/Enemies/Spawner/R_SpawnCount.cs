using Godot;
using System;

[GlobalClass]
public partial class R_SpawnCount : Resource
{
    [Export] public Data_Enemy Data;
    [Export] public Vector2I WaveRange = new(1,0);
    public int Count = 0;

    public R_SpawnCount() {}
    public R_SpawnCount(R_SpawnCount other)
    {
        Data = other.Data;
        WaveRange = other.WaveRange;
        Count = 0;
    }
}