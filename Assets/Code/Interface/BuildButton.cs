using Godot;
using System;

public partial class BuildButton : Node2D
{
	[Export] public R_BuildTower BuildParams = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnClick()
	{
		if (BuildParams == null) return;
		if (BuildParams.TowerToBuild == null) return;

		if (Player.CanAfford(BuildParams) == false) return;
        if (Cursor.PushState("State_Placement") is S_PlaceTower PlacementState)
        {
            PlacementState.SetTowerToBuild(BuildParams);
        }
    }
}
