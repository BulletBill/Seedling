using Godot;
using System;
using Godot.Collections;

public interface ITowerComponent
{
	void TowerReady();
	void TowerRemoved();
}

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

		foreach(Node n in GetChildren())
		{
			if (n is ITowerComponent towerComp)
			{
				towerComp.TowerReady();
			}
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
		if (Index < 0 || Index >= Actions.Count) return;

		switch (Actions[Index].ActionType)
		{
			case EActionType.Sell:
				PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
				Cursor.PopState();
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
		if (TowerData == null || TowerData.Cost == null) return;
		R_Cost Refund = TowerData.Cost * 0.80f;
		if (Refund.LifeForce > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddLifeforce, Refund.LifeForce);
		}
		if (Refund.Substance > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddSubstance, Refund.Substance);
		}
		if (Refund.Flow > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddFlow, Refund.Flow);
		}
		if (Refund.Breath > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddBreath, Refund.Breath);
		}
		if (Refund.Energy > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddEnergy, Refund.Energy);
		}
		EffectsManager.SpawnResourceCluster(GlobalPosition, Refund.Substance, Refund.Flow, Refund.Breath, Refund.Energy);

		foreach(Node n in GetChildren())
		{
			if (n is ITowerComponent towerComp)
			{
				towerComp.TowerRemoved();
			}
		}

		QueueFree();
	}

	public void UpgradeTo(Tower NewTower)
	{

	}
}