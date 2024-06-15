using Godot;
using System;
using Godot.Collections;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public partial class Tower : Node2D, IHoverable
{
	public static readonly String GroupName = "Tower";
	[Export] public bool IsDefendTarget = false;
	[Export] public Data_Tower TowerData;
	[Export] public Array<Data_Action> Actions = new();
	[Export] public Array<Data_Action> UpgradingActions = new();
	public Vector2I MapPosition = new();
	public R_Cost TotalCost = new();
	
	// Upgrade/Build variables
	[Export] public double BuildTime = 0.0f;
	protected ProgressBar TimerBar;
	protected Data_Tower UpgradeData = null;
	public double BuildTimer { get; protected set; }
	public bool Building { get; protected set; } = false;
	public bool Upgrading { get; protected set; } = false;

	// Leveling
	public int TowerLevel { get; protected set; } = 1;


	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => Deselected()));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };
		TimerBar = GetNodeOrNull<ProgressBar>("BuildBar");

		if (TowerData == null)
		{
			QueueFree();
			return;
		}
		
		TotalCost += TowerData.Cost;

		if (IsDefendTarget)
		{
			Player.DefendTargets.Add(this);
		}

		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
			hoverArea.ReactStates.Add(ECursorState.Free);
			hoverArea.ReactStates.Add(ECursorState.Menu_Context);
		}

		foreach(Node n in GetChildren())
		{
			if (n is TowerComponent towerComp)
			{
				towerComp.TowerReady();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (BuildTimer > 0.0f)
		{
			BuildTimer -= delta * Level.GetSpeed();
			if (IsInstanceValid(TimerBar))
			{
				TimerBar.Value = TimerBar.MaxValue - (int)((BuildTimer / BuildTime) * TimerBar.MaxValue);
			}
			
			if (BuildTimer <= 0.0f || Player.Singleton.FreeTowers)
			{
				// Finish upgrading
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
		Cursor.Broadcast(Cursor.SignalName.SelectableHoveredCustom, TowerData.Icon, TowerData.DisplayName, TowerData.GetLeveledDescription(TowerLevel));
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
	}

	public void SellTower()
	{
		R_Cost Refund = null;

		if (Upgrading)
		{
			Refund = UpgradeData.Cost;
		}
		else if (Building)
		{
			Refund = TotalCost;
		}
		else
		{
			Refund = TotalCost * 0.50f;
		}

		if (Refund != null)
		{
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
		}

		if (!Upgrading)
		{
			foreach(Node n in GetChildren())
			{
				if (n is TowerComponent towerComp)
				{
					towerComp.TowerRemoved();
				}
			}
			PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerRemoved, this);
			QueueFree();
		}
		else
		{
			Upgrading = false;
			BuildTimer = 0.0f;
			if (IsInstanceValid(TimerBar)) { TimerBar.Visible = false; }

			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("Animator");
			Anim?.Play("RESET");
		}
	}

	public void SetData(Data_Tower NewData)
	{
		if (NewData == null) return;

		TowerData = NewData;
		Sprite2D Image = GetNodeOrNull<Sprite2D>("Image");
		Sprite2D Shadow = GetNodeOrNull<Sprite2D>("Shadow");

		if (IsInstanceValid(Image))
		{
			Image.Texture = TowerData.Icon;
		}
		if (IsInstanceValid(Shadow))
		{
			Shadow.Texture = TowerData.Icon;
		}

		foreach (PackedScene SceneData in NewData.ExtraBehaviors)
		{
			TowerComponent NewComponent = SceneData.InstantiateOrNull<TowerComponent>();
			if (NewComponent != null)
			{
				AddChild(NewComponent);
			}
		}

		foreach (Node child in GetChildren())
		{
			if (child is TowerComponent component)
			{
				component.TowerUpdated();
			}
		}
	}
}