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
    [Export] public PackedScene SceneToCreate { get; protected set; } = null;
    [Export] public int DesiredPosition { get; protected set; } = 0; 

    public void SetFromTowerParams()
    {
        if (TowerData == null) return;
        DisplayName = TowerData.DisplayName;
        Description = TowerData.Description;
        Icon = TowerData.Icon;

        ClickCost = TowerData.Cost;
    }

    public void Clear()
    {
        ActionType = EActionType.None;
        ClickCost = new();
        TowerData = null;
        SceneToCreate = null;
        DesiredPosition = 0;

        Icon = null;
        DisplayName = "";
        Description = "";
    }

    public override string GetFullDescription()
    {
        //String CombinedDescription = ClickCost.GetFullString() + "\n";
        //CombinedDescription += Description;

        return Description;
    }
}
