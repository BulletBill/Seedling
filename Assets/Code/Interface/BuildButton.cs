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

		if (BuildParams != null)
		{
			Sprite2D TowerSprite = GetNodeOrNull<Sprite2D>("TowerSprite");
			if (TowerSprite != null)
			{
				TowerSprite.Texture = BuildParams.PlacementSprite;
			}
		}

		Sprite2D Outline = GetNodeOrNull<Sprite2D>("Outline");
		if (Outline != null)
		{
			Outline.Material = new ShaderMaterial() { Shader = (Outline.Material as ShaderMaterial).Shader.Duplicate() as Shader };
			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			Anim?.Play("Unhover");
		}

		CostReadout CostText = GetNodeOrNull<CostReadout>("Cost");
		if (CostText != null && BuildParams != null)
		{
			CostText.SetCosts(BuildParams.Cost);
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
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		if (Player.CanAfford(BuildParams.Cost) == false)
		{
			Anim?.Play("Error");
			return;
		}
        if (Cursor.PushState("State_Placement") is S_PlaceTower PlacementState)
        {
			Anim?.Play("Success");
            PlacementState.SetTowerToBuild(BuildParams);
        }
    }
}
