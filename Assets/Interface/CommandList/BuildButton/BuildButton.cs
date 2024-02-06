using Godot;
using System;

public partial class BuildButton : Node2D, IHoverable
{
	[Export] public PackedScene TowerToBuild;
	[Export] public Data_Tower BuildParams = new();
	bool CanAfford = false;
	HoverArea HoverArea;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (HoverArea != null)
		{
			HoverArea.Clicked += OnClick;
		}

		Sprite2D Outline = GetNodeOrNull<Sprite2D>("Outline");
		if (Outline != null)
		{
			Outline.Material = new ShaderMaterial() { Shader = (Outline.Material as ShaderMaterial).Shader.Duplicate() as Shader };
			AnimationPlayer HoverAnim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			HoverAnim?.Play("Unhover");
		}

		if (TowerToBuild == null)
		{
			Disable();
			return;
		}

		AssignBuildParams(BuildParams);
		PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateCosts()));
		CanAfford = Player.CanAfford(BuildParams.Cost);
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (CanAfford)
		{
			Anim?.Play("Enabled");
			HoverArea?.SetDisabled(false);
		}
		else
		{
			Anim?.Play("Disabled");
			HoverArea?.SetDisabled(true);
		}
	}

	public void AssignBuildParams(Data_Tower NewParams)
	{
		BuildParams = NewParams;
		if (NewParams == null) { Disable(); return; }

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

		RichTextLabel NameText = GetNodeOrNull<RichTextLabel>("Name");
		if (NameText != null)
		{
			NameText.Text = TextHelpers.Center(NewParams.TowerName);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateCosts()
	{
		if (BuildParams == null) return;
		bool CanAffordNow = Player.CanAfford(BuildParams.Cost);
		if (CanAfford == CanAffordNow) return;

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		CanAfford = CanAffordNow;
		if (CanAfford)
		{
			Anim?.Play("Enabled");
			HoverArea?.SetDisabled(false);
		}
		else
		{
			Anim?.Play("Disabled");
			HoverArea?.SetDisabled(true);
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

	void Disable()
	{
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Disabled");
		HoverArea?.SetDisabled(true);

		Sprite2D TowerSprite = GetNodeOrNull<Sprite2D>("TowerSprite");
		if (TowerSprite != null)
		{
			TowerSprite.Texture = null;
		}
	}

	public void OnHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerHovered, BuildParams);
	}

	public void ExitHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerExitHovered);
	}
}
