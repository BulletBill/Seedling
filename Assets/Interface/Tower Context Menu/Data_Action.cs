using Godot;
using System;

public enum EActionType
{
    None,
    Cancel,
    Sell,
    SelfUpgrade,
    StatUpgrade,
}

[GlobalClass]
public partial class Data_Action : Resource
{
    [Export] public String DisplayName;
    [Export] public EActionType ActionType;
    [Export] public R_Cost ClickCost = new R_Cost();
    [Export] public Texture2D Icon = null;
    [Export] public PackedScene UpgradeScene = null;
}
