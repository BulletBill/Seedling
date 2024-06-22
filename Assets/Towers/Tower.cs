using Godot;
using System;
using Godot.Collections;

public partial class Tower : Node2D, IHoverable
{
	public static readonly String GroupName = "Tower";
	[Export] public bool IsDefendTarget = false;
	[Export] public Data_Tower TowerData;
	[Export] public Array<Data_Action> Actions = new();
	[Export] public Array<Data_Action> UpgradingActions = new();
	[Export] public Array<Data_Action> BuildingActions = new();
	public Vector2I MapPosition = new();
	public R_Cost TotalCost = new();
	
	// Upgrade/Build variables
	protected ProgressBar TimerBar;
	public double BuildTimer { get; protected set; }
	public bool Building { get; protected set; } = false;
	public bool Upgrading { get; protected set; } = false;

	// Leveling
	public int TowerLevel { get; protected set; } = 1;

	[Signal] public delegate void TowerUpdatedEventHandler();


	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => Deselected()));
		MainMap.Register(MainMap.SignalName.GridVisibleChanged, Callable.From((bool v) => ShowDetailsChanged(v)));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };

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
			if (IsInstanceValid(TimerBar) && TowerData != null)
			{
				TimerBar.Value = TimerBar.MaxValue - (int)((BuildTimer / TowerData.BuildTime) * TimerBar.MaxValue);
			}
			
			if (BuildTimer <= 0.0f || Player.Singleton.FreeTowers)
			{
				if (Building)
				{
					FinishBuild();
				}
				if (Upgrading)
				{
					FinishUpgrade();
				}
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
		Texture2D Image = TowerData.Icon;
		String Header = TowerLevel > 1 ? TowerData.DisplayName + " Level " + TowerLevel.ToString() : TowerData.DisplayName;
		String Body = TowerData.GetLeveledDescription(TowerLevel);
		Data_Hoverable UpdatedData = new(Header, Image, Body);
		Cursor.Broadcast(Cursor.SignalName.SelectableHovered, UpdatedData);
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

			SetSelectedText();

			PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerSelected, this);
			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
			Anim?.Play("Hover");
        }
	}

	public void SetSelectedText()
	{
		Texture2D Image = TowerData.Icon;
		String Header = TowerLevel > 1 ? TowerData.DisplayName + " Level " + TowerLevel.ToString() : TowerData.DisplayName;
		String Body = TowerData.GetLeveledDescription(TowerLevel);
		Data_Hoverable UpdatedData = new(Header, Image, Body);
		Cursor.Broadcast(Cursor.SignalName.SetFixedObject, UpdatedData);
	}

	void Deselected()
	{
	}

	public void ShowDetailsChanged(bool ShowDetail)
	{
		RichTextLabel LevelLabel = GetNodeOrNull<RichTextLabel>("LevelLabel");
		if (IsInstanceValid(LevelLabel))
		{
			LevelLabel.Visible = ShowDetail && TowerLevel > 1;
		}
	}

	public void SellTower()
	{
		R_Cost Refund = null;

		if (Upgrading)
		{
			Refund = TowerData.UpgradeCostPerLevel * (TowerLevel + 1);
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
			BuildTimer = 0.0f;
			if (IsInstanceValid(TimerBar)) { TimerBar.Visible = false; }

			AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("Animator");
			Anim?.Play("RESET");

			Upgrading = false;
			EmitSignal(SignalName.TowerUpdated);
		}
	}

	public void StartBuild(Data_Tower NewData)
	{
		if (NewData == null) return;

		TowerData = NewData;
		Sprite2D Image = GetNodeOrNull<Sprite2D>("Image");
		Sprite2D Shadow = GetNodeOrNull<Sprite2D>("Shadow");
		TimerBar = GetNodeOrNull<ProgressBar>("BuildBar");
	
		if (IsInstanceValid(Image))
		{
			Image.Texture = TowerData.SpriteSheet;
		}
		if (IsInstanceValid(Shadow))
		{
			Shadow.Visible = false;
		}
		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Visible = true;
			TimerBar.MaxValue = (int)(TowerData.BuildTime * 100);
		}

		BuildTimer = TowerData.BuildTime;

		Building = true;
		EmitSignal(SignalName.TowerUpdated);
	}

	public void FinishBuild()
	{
		if (Building == false)
		{
			Game.LogError(LogCategory.Tower, "Already finished building!");
			return;
		}
		if (TowerData == null)
		{
			Game.LogError(LogCategory.Tower, "Tried to finish building with no data!");
			return;
		}

		Sprite2D Image = GetNodeOrNull<Sprite2D>("Image");
		Sprite2D Shadow = GetNodeOrNull<Sprite2D>("Shadow");

		if (IsInstanceValid(Image))
		{
			Image.Texture = TowerData.Icon;
		}
		if (IsInstanceValid(Shadow))
		{
			Shadow.Texture = TowerData.Icon;
			Shadow.Visible = true;
		}
		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Visible = false;
		}

		//Create new instance of action list to break references
		Array<Data_Action> NewActionList = new();
		foreach (Data_Action CurrentAction in Actions)
		{
			NewActionList.Add(CurrentAction);
		}

		foreach (Data_Action AddAction in TowerData.ExtraActions)
		{
			Data_Action NewAction = new(AddAction);
			NewActionList.Add(NewAction);
			if (NewAction.ActionType == EActionType.SelfUpgrade)
			{
				NewAction.SetCost(GetUpgradeCost());
			}
		}
		Actions = NewActionList;

		foreach (PackedScene SceneData in TowerData.ExtraBehaviors)
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

		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerFinished, this);
		
		Building = false;
		EmitSignal(SignalName.TowerUpdated);
	}

	public void StartUpgrade()
	{
		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Visible = true;
		}
		BuildTimer = TowerData.BuildTime;

		Upgrading = true;
		EmitSignal(SignalName.TowerUpdated);
	}

	public void FinishUpgrade()
	{
		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Visible = false;
		}

		TotalCost += GetUpgradeCost();
		TowerLevel += 1;

		for (int i = Actions.Count - 1; i >= 0; i--)
		{
			if (Actions[i].ActionType == EActionType.SelfUpgrade)
			{
				if (TowerLevel >= TowerData.MaximumLevel)
				{
					Actions.RemoveAt(i);
				}
				else
				{
					Actions[i].SetCost(GetUpgradeCost());
				}
			}
		}

		foreach (Node child in GetChildren())
		{
			if (child is TowerComponent component)
			{
				component.TowerUpdated();
			}
		}

		RichTextLabel LevelLabel = GetNodeOrNull<RichTextLabel>("LevelLabel");
		if (IsInstanceValid(LevelLabel))
		{
			LevelLabel.Text = TowerLevel.ToString();
		}

		Upgrading = false;
		EmitSignal(SignalName.TowerUpdated);
	}

	public R_Cost GetUpgradeCost()
	{
		R_Cost ret = new(TowerData.UpgradeCostPerLevel);
		ret *= TowerLevel + 1;

		return ret;
	}
}