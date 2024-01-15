using Godot;
using System;

public enum EBuildRequirement
{
    None,
    GrassOnly,
    Rock,
    Water,
    Chasm,
    Lava,
}

public partial class Data_Tower : Resource
{
    [Export] public Texture2D PlacementSprite;
    [Export] public String TowerName;
    [Export(PropertyHint.MultilineText)] public String Description { get; protected set; }
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
