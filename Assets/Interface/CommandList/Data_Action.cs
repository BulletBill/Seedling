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
    [Export] public EActionType ActionType { get; protected set; } = EActionType.None;
    [Export] public R_Cost ClickCost { get; protected set; } = new();
    [Export] public Data_Tower TowerData { get; protected set; } = null;
    [Export] public int DesiredPosition { get; protected set; } = 0;
    public bool Disabled = false;

    public Data_Action(Data_Action OtherAction)
    {
        ActionType = OtherAction.ActionType;
        ClickCost = OtherAction.ClickCost;
        TowerData = OtherAction.TowerData;
        DesiredPosition = OtherAction.DesiredPosition;
        Icon = OtherAction.Icon;
        Description = OtherAction.Description;
        DisplayName = OtherAction.DisplayName;
    }

    public Data_Action() {}

    public void SetFromTowerParams()
    {
        if (TowerData == null) return;
        DisplayName = TowerData.DisplayName;
        Description = TowerData.GetFullDescription();
        Icon = TowerData.Icon;

        ClickCost = TowerData.Cost;
    }

    public void SetCost(R_Cost NewCost)
    {
        ClickCost = NewCost;
    }

    public void Clear()
    {
        ActionType = EActionType.None;
        ClickCost = new();
        TowerData = null;
        DesiredPosition = 0;

        Icon = null;
        DisplayName = "";
        Description = "";
    }

    public override string GetFullDescription()
    {
        return Description;
    }
}
