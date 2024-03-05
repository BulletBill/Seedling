using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Data_Tower : Data_Hoverable
{
    [Export] public String Ident;
    [Export] public R_Cost Cost { get; protected set; } = new();
    [Export] public float BuildTime { get; protected set; } = 0.0f;
    [Export] public bool NeedsSparkFlag { get; protected set; } = false;
    [Export] public bool NeedsGrassFlag { get; protected set; } = true;
    [Export] public bool CanBuildOnGrass { get; protected set; } = true;
    [Export] public bool CanBuildOnDirt { get; protected set; } = false;
    [Export] public bool CanBuildOnWater { get; protected set; } = false;
    [Export] public bool CanBuildOnChasm { get; protected set; } = false;
    [Export] public bool CanBuildOnStone { get; protected set; } = false;

    public override string GetFullDescription()
    {
        return Description;
    }
}
