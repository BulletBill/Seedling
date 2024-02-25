using Godot;
using System;

[GlobalClass]
public partial class R_SpawnCount : Resource
{
    [Export] public R_Spawn SpawnData;
    [Export] public int Count = 0;
}