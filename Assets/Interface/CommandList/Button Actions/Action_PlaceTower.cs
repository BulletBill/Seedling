using Godot;
using System;

public partial class Action_PlaceTower : Node, IButtonAction
{
    [Export] public PackedScene TowerToBuild;
	[Export] public Data_Tower BuildParams = new();
    public void Execute()
    {
        if (Cursor.PushState("State_Placement") is S_PlaceTower PlacementState)
        {
            PlacementState.SetTowerToBuild(BuildParams, TowerToBuild);
        }
    }
}
