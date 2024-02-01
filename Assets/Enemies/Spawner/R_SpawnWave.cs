using Godot;
using System;

[GlobalClass]
public partial class R_SpawnWave : Resource
{
    [Export] public Godot.Collections.Array<R_SpawnCount> SpawnCounts = new();
}
