using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Data_Tower : Data_Hoverable
{
    [Export] public String Ident;
    [Export] public R_Cost Cost { get; protected set; } = new();
    [Export] public float BuildTime { get; protected set; } = 0.0f;
    [Export] public int MinDamage { get; protected set; } = 0;
    [Export] public int MaxDamage { get; protected set; } = 0;
    [Export] public float Range { get; protected set; } = 0.0f;
    [Export] public float AreaOfEffect { get; protected set; } = 0.0f;
    [Export] public float FireDelay { get; protected set; } = 1.0f;
    [Export] public bool NeedsSparkFlag { get; protected set; } = false;
    [Export] public bool NeedsGrassFlag { get; protected set; } = true;
    [Export] public bool CanBuildOnGrass { get; protected set; } = true;
    [Export] public bool CanBuildOnDirt { get; protected set; } = false;
    [Export] public bool CanBuildOnWater { get; protected set; } = false;
    [Export] public bool CanBuildOnChasm { get; protected set; } = false;
    [Export] public bool CanBuildOnStone { get; protected set; } = false;

    public override string GetFullDescription()
    {
        string ret = Description;
        ret = ret.ReplaceN("%bt", BuildTime.ToString());

        ret = ret.ReplaceN("%min", MinDamage.ToString());
        ret = ret.ReplaceN("%max", MaxDamage.ToString());
        ret = ret.ReplaceN("%rng", Range.ToString());
        ret = ret.ReplaceN("%aoe", AreaOfEffect.ToString());
        ret = ret.ReplaceN("%rof", FireDelay.ToString());

        return ret;
    }
}
