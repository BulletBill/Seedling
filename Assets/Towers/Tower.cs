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

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => Deselected()));
	}

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
		Cursor.Broadcast(Cursor.SignalName.SelectableHovered, TowerData);
	}

	public void ExitHovered()
	{
		Cursor.Broadcast(Cursor.SignalName.SelectableExited);
	}

	void OnClick()
	{
		if (Actions.Count <= 0) return;

		if (Cursor.PushState("State_ContextMenu") is S_ContextMenu ContextState)
        {
            ContextState.AssignTower(this);
			Cursor.Broadcast(Cursor.SignalName.SetFixedObject, TowerData);
			PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerSelected, this);
			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			Anim?.Play("Hover");
        }
	}

	void Deselected()
	{
		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null && !hoverArea.Hovered && Cursor.GetSelectedObject() == this)
		{
			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			Anim?.Play("Unhover");
		}
	}

	public void SellTower()
	{
		if (TowerData == null || TowerData.Cost == null) return;
		R_Cost Refund = TowerData.Cost * 0.50f;
		if (Refund.LifeForce > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddLifeforce, -TowerData.Cost.LifeForce);
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