using Godot;
using System;
using System.Globalization;

public partial class ActionButton : Node2D, IHoverable
{
	[Export] public int ActionIndex = 0;
	[Export] public Data_Action ActionParams = new();
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

		PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateCosts()));
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Disabled");

		AssignActionParams(ActionParams);
	}

	public void AssignActionParams(Data_Action NewParams)
	{
		if (NewParams == null)
		{
			Modulate = new Color(0,0,0,0);
			return;
		} 

		ActionParams = NewParams;
		Modulate = new Color(1,1,1,1);

		Sprite2D Icon = GetNodeOrNull<Sprite2D>("Icon");
		if (Icon != null)
		{
			Icon.Texture = ActionParams.Icon;
		}

		CostReadout CostText = GetNodeOrNull<CostReadout>("Cost");
		if (CostText != null && ActionParams != null && ActionParams.ClickCost != null)
		{
			CostText.SetCosts(ActionParams.ClickCost);
		}

		RichTextLabel NameText = GetNodeOrNull<RichTextLabel>("Name");
		if (NameText != null)
		{
			NameText.Text = TextHelpers.Center(NewParams.DisplayName);
		}
		UpdateCosts();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateCosts()
	{
		if (ActionParams == null) return;
		bool CanAffordNow = Player.CanAfford(ActionParams.ClickCost);
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
		ContextMenu ParentMenu = GetParentOrNull<ContextMenu>();
		if (ParentMenu == null) return;
		if (ParentMenu.SelectedTower == null) return;

		ParentMenu.SelectedTower.PerformAction(ActionIndex);
    }

	public void OnHovered()
	{
		//PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerHovered, BuildParams);
	}

	public void ExitHovered()
	{
		//PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerExitHovered);
	}
}
