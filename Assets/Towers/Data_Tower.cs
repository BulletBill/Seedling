using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Data_Tower : Data_Hoverable
{
    [Export] public PackedScene TowerBuildTemplate;
    [Export] public String Ident;
    [Export] public R_Cost Cost { get; protected set; } = new();
    [Export] public R_Cost UpgradeCostPerLevel { get; protected set; } = new();
    [Export] public float BuildTime { get; protected set; } = 0.0f;
    [Export] public int MinDamage { get; protected set; } = 0;
    [Export] public int MaxDamage { get; protected set; } = 0;
    [Export] public float Range { get; protected set; } = 0.0f;
    [Export] public float AreaOfEffect { get; protected set; } = 0.0f;
    [Export] public float FireDelay { get; protected set; } = 1.0f;
    [Export] public int MaximumLevel { get; protected set; } = 1;
    [Export] public Texture2D UpgradeSprite { get; protected set; } = null;
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

    public string GetLeveledDescription(int TowerLevel)
    {
        int LeveledMinDamage = Mathf.RoundToInt(MathHelper.LeveledDamageFormula(MinDamage, TowerLevel));
        int LeveledMaxDamage = Mathf.RoundToInt(MathHelper.LeveledDamageFormula(MaxDamage, TowerLevel));

        string ret = Description;
        ret = ret.ReplaceN("%bt", BuildTime.ToString());

        ret = ret.ReplaceN("%min", LeveledMinDamage.ToString());
        ret = ret.ReplaceN("%max", LeveledMaxDamage.ToString());
        ret = ret.ReplaceN("%rng", Range.ToString());
        ret = ret.ReplaceN("%aoe", AreaOfEffect.ToString());
        ret = ret.ReplaceN("%rof", FireDelay.ToString());

        return ret;
    }

    public Tower_Build CreateTower()
    {
        Tower_Build NewTower = TowerBuildTemplate.InstantiateOrNull<Tower_Build>();
        if (IsInstanceValid(NewTower))
        {
            NewTower.SetData(this);
        }

        return NewTower;
    }
}
