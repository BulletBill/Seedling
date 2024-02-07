using Godot;
using System;

public partial class CommandButton : Node2D, IHoverable
{
	[Export] public Data_Tower TowerParams = null;
	[Export] public Data_Action ActionParams = new();
	[Export] public String CursorState = "Free";
	bool CanAfford = false;
	HoverArea HoverArea;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (TowerParams != null)
		{
			ActionParams.SetFromTowerParams(TowerParams);
		}

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

		if (ActionParams.ActionType == EActionType.None)
		{
			Disable();
			return;
		}

		AssignActionParams(ActionParams);
		PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateCosts()));
		CanAfford = Player.CanAfford(ActionParams.ClickCost);
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

	public void AssignActionParams(Data_Action NewParams)
	{
		if (NewParams == null) { Disable(); return; }

		Sprite2D TowerSprite = GetNodeOrNull<Sprite2D>("TowerSprite");
		if (TowerSprite != null)
		{
			TowerSprite.Texture = NewParams.Icon;
		}

		CostReadout CostText = GetNodeOrNull<CostReadout>("Cost");
		if (CostText != null && ActionParams.ActionType != EActionType.None)
		{
			CostText.SetCosts(ActionParams.ClickCost);
		}

		RichTextLabel NameText = GetNodeOrNull<RichTextLabel>("Name");
		if (NameText != null)
		{
			NameText.Text = TextHelpers.Center(NewParams.DisplayName);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateCosts()
	{
		if (ActionParams.ActionType == EActionType.None) return;
		bool CanAffordNow = Player.CanAfford(ActionParams.ClickCost);
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
		if (ActionParams == null) return;
		if (!CanAfford) return;

		foreach (Node n in GetChildren())
		{
			if (n is IButtonAction buttonAction)
			{
				buttonAction?.Execute();
			}
		}

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Success");
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
		if (ActionParams.TowerData != null)
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerHovered, ActionParams.TowerData);
	}

	public void ExitHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerExitHovered);
	}
}


public interface IButtonAction
{
	public void Execute();
}