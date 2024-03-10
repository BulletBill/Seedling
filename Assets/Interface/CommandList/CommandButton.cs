using Godot;
using System;

public partial class CommandButton : Node2D, IHoverable
{
	[Export] public Data_Action ActionParams = new();
	bool CanAfford = false;
	bool HasCost = false;
	bool Hovered = false;
	public bool Disabled { get; protected set; } = true;
	HoverArea HoverArea;

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateCosts(false)));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActionParams?.SetFromTowerParams();

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

		AssignActionParams(ActionParams);
	}

	public void AssignActionParams(Data_Action NewParams)
	{
		if (NewParams == null)
		{
			ActionParams = null;
		}
		else
		{
			ActionParams = NewParams;
			ActionParams.SetFromTowerParams();
		}

		if (ActionParams == null || ActionParams.ActionType == EActionType.None)
		{
			Disable();
			return;
		}
		
		Disabled = false;

		Sprite2D IconSprite = GetNodeOrNull<Sprite2D>("Icon");
		if (IconSprite != null)
		{
			IconSprite.Texture = ActionParams.Icon;
		}

		CostReadout CostText = GetNodeOrNull<CostReadout>("Cost/Cost Text");
		if (CostText != null && ActionParams.ActionType != EActionType.None)
		{
			CostText.SetCosts(ActionParams.ClickCost);
		}

		RichTextLabel NameText = GetNodeOrNull<RichTextLabel>("Name");
		if (NameText != null)
		{
			NameText.Text = TextHelpers.Center(ActionParams.DisplayName);
		}

		UpdateCosts(true);
	}

	void UpdateCosts(bool ForceUpdate)
	{
		HasCost = false;
		if (ActionParams == null || ActionParams.ActionType == EActionType.None) return;
		HasCost = !ActionParams.ClickCost.IsZero();
		bool CanAffordNow = Player.CanAfford(ActionParams.ClickCost);
		if (CanAfford == CanAffordNow && !ForceUpdate) return;

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

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Success");

		switch (ActionParams.ActionType)
		{
			case EActionType.Build:
				Execute_BuildTower();
			break;
			case EActionType.Cancel:
				Cursor.PopState();
			break;
			case EActionType.Sell:
				Execute_SellTower();
			break;
			case EActionType.SelfUpgrade:
			break;
			case EActionType.StatUpgrade:
			break;
			case EActionType.None:
			break;
		}
    }
	
	void Disable()
	{
		Disabled = true;
		HasCost = false;
		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("Disabled");
		HoverArea?.SetDisabled(true);

		Sprite2D IconSprite = GetNodeOrNull<Sprite2D>("Icon");
		if (IconSprite != null)
		{
			IconSprite.Texture = null;
		}
	}

	public void OnHovered()
	{
		Hovered = true;
		Cursor.Broadcast(Cursor.SignalName.SelectableHovered, ActionParams);
		if (HasCost)
		{
			NinePatchRect CostContainer = GetNodeOrNull<NinePatchRect>("Cost");
			if (CostContainer != null)
			{
				CostContainer.Visible = true;
			}
		}
	}

	public void ExitHovered()
	{
		Hovered = false;
		Cursor.Broadcast(Cursor.SignalName.SelectableExited);

		NinePatchRect CostContainer = GetNodeOrNull<NinePatchRect>("Cost");
		if (CostContainer != null)
		{
			CostContainer.Visible = false;
		}
	}

	void Execute_BuildTower()
	{
		if (ActionParams == null || ActionParams.SceneToCreate == null || ActionParams.TowerData == null) return;
		Data_Tower OutTower = ActionParams.TowerData;
		PackedScene OutScene = ActionParams.SceneToCreate;

        if (Cursor.PushState("State_Placement") is S_PlaceTower PlacementState)
        {
            PlacementState.SetTowerToBuild(OutTower, OutScene);
        }
	}

	static void Execute_SellTower()
	{
		if (Cursor.GetSelectedObject() is Tower SelectedTower)
		{
			SelectedTower.SellTower();
			PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
			Cursor.PopState();
		}
	}
}


public interface IButtonAction
{
	public void Execute();
}