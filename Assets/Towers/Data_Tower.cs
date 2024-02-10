using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Data_Tower : Data_Hoverable
{
    [Export] public R_Cost Cost = new();
    [Export] public float BuildTime;
    [Export] public bool NeedsSparkFlag = false;
    [Export] public bool NeedsGrassFlag = true;
    [Export] public bool CanBuildOnGrass = true;
    [Export] public bool CanBuildOnDirt = false;
    [Export] public bool CanBuildOnWater = false;
    [Export] public bool CanBuildOnChasm = false;
    [Export] public bool CanBuildOnStone = false;
}
