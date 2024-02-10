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
public partial class Data_Action : Data_Hoverable
{
    [Export] public EActionType ActionType = EActionType.None;
    [Export] public R_Cost ClickCost = new();
    [Export] public Data_Tower TowerData = null;

    public void SetFromTowerParams(Data_Tower TowerParams)
    {
        DisplayName = TowerParams.DisplayName;
        Description = TowerParams.Description;
        Icon = TowerParams.Icon;

        ClickCost = TowerParams.Cost;
        TowerData = TowerParams;
    }
}
