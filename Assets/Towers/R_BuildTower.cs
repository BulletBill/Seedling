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

public partial class R_BuildTower : Resource
{
    [Export] public Texture2D PlacementSprite;
    [Export] public PackedScene TowerToBuild;
    [Export] public String TowerName;
    [Export(PropertyHint.MultilineText)] public String Description { get; protected set; }
    [Export] public R_Cost Cost = new();
    [Export] public float BuildTime;
    [Export] public bool NeedsGrass = true;
    //[Export] public bool NeedsWater = false;
}