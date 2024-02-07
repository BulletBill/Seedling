using Godot;
using System;

public enum EActionType
{
    None,
    Build,
    Cancel,
    Sell,
    SelfUpgrade,
    StatUpgrade,
}

[GlobalClass]
public partial class Data_Action : Resource
{
    [Export] public String DisplayName = "No Name";
    [Export] public EActionType ActionType = EActionType.None;
    [Export] public R_Cost ClickCost = new();
    [Export] public Texture2D Icon = null;
    [Export] public Data_Tower TowerData = null;

    public void SetFromTowerParams(Data_Tower TowerParams)
    {
        DisplayName = TowerParams.TowerName;
        ClickCost = TowerParams.Cost;
        Icon = TowerParams.PlacementSprite;
        TowerData = TowerParams;
    }
}
