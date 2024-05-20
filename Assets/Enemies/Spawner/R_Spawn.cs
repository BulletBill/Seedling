using Godot;
using System;

[GlobalClass]
public partial class R_Spawn : Resource
{
    [Export] public PackedScene EnemyScene;
    [Export] public Data_Enemy Data;
    [Export] public Vector2I WaveRange = new(1,0);
}