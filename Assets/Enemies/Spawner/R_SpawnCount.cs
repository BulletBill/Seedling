using Godot;
using System;

[GlobalClass]
public partial class R_SpawnCount : Resource
{
    [Export] public PackedScene EnemyScene;
    [Export] public int Count = 0;
}
