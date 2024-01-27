using Godot;
using System;
using Godot.Collections;

public partial class Tower : Sprite2D, IHoverable
{
	[Export] public bool IsDefendTarget = false;
	[Export] public Data_Tower TowerData;
	[Export] public Array<Data_Action> Actions = new();
	public static readonly String GroupName = "Tower";
	public Vector2I MapPosition = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };

		if (IsDefendTarget)
		{
			Player.DefendTargets.Add(this);
		}

		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
		}
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		Player.DefendTargets.Remove(this);
    }

	public void OnHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerHovered, TowerData);
	}

	public void ExitHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerExitHovered);
	}

	void OnClick()
	{
		if (Actions.Count <= 0) return;

		if (Cursor.PushState("State_ContextMenu") is S_ContextMenu ContextState)
        {
            ContextState.AssignTower(this);

			//Make tower look selected
        }
	}

	public void PerformAction(int Index)
	{
		if (Index < 0 || Actions.Count >= Index) return;

		switch (Actions[Index].ActionType)
		{
			case EActionType.Sell:
				SellTower();
			break;
			case EActionType.SelfUpgrade:
				if (Actions[Index].UpgradeScene != null)
				{
					UpgradeTo(Actions[Index].UpgradeScene.Instantiate<Tower>());
				}
			break;
			case EActionType.StatUpgrade:
			break;
			case EActionType.Cancel:
			break;
			default:
			break;
		}
	}

	public void SellTower()
	{

	}

	public void UpgradeTo(Tower NewTower)
	{

	}
}