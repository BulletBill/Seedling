using Godot;
using System;

public partial class BuildButton : Node2D
{
	[Export] public PackedScene TowerToBuild;
	[Export] public Data_Tower BuildParams = new();
	bool CanAfford = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
		}

		Sprite2D Outline = GetNodeOrNull<Sprite2D>("Outline");
		if (Outline != null)
		{
			Outline.Material = new ShaderMaterial() { Shader = (Outline.Material as ShaderMaterial).Shader.Duplicate() as Shader };
			AnimationPlayer HoverAnim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			HoverAnim?.Play("Unhover");
		}

		AssignBuildParams(BuildParams);
		PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateCosts()));
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Disabled");
	}

	public void AssignBuildParams(Data_Tower NewParams)
	{
		if (NewParams == null) return;
		BuildParams = NewParams;

		Sprite2D TowerSprite = GetNodeOrNull<Sprite2D>("TowerSprite");
		if (TowerSprite != null)
		{
			TowerSprite.Texture = BuildParams.PlacementSprite;
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

	public void UpdateCosts()
	{
		bool CanAffordNow = Player.CanAfford(BuildParams.Cost);
		if (CanAfford == CanAffordNow) return;

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		CanAfford = CanAffordNow;
		if (CanAfford)
		{
			Anim?.Play("Enabled");
		}
		else
		{
			Anim?.Play("Disabled");
		}
	}

	void OnClick()
	{
		if (BuildParams == null) return;
		if (TowerToBuild == null) return;

        if (CanAfford && Cursor.PushState("State_Placement") is S_PlaceTower PlacementState)
        {
            PlacementState.SetTowerToBuild(BuildParams, TowerToBuild);

			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
			Anim?.Play("Success");
        }
    }
}