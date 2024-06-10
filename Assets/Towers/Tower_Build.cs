using Godot;
using System;

public partial class Tower_Build : Node2D, IHoverable
{
	[Export] public PackedScene TowerTemplate;
	[Export] public Data_Tower TowerData;
	double BuildTimer;
	protected ProgressBar TimerBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };
		TimerBar = GetNodeOrNull<ProgressBar>("TimerBar");

		BuildTimer = TowerData.BuildTime;
		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Visible = true;
			TimerBar.MaxValue = (int)(TowerData.BuildTime * 100);
		}
		
		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
			hoverArea.ReactStates.Add(ECursorState.Free);
			hoverArea.ReactStates.Add(ECursorState.Menu_Context);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (BuildTimer > 0.0f)
		{
			BuildTimer -= delta * Level.GetSpeed();
			if (IsInstanceValid(TimerBar))
			{
				TimerBar.Value = TimerBar.MaxValue - (int)((BuildTimer / TowerData.BuildTime) * TimerBar.MaxValue);
			}
			
			if (BuildTimer <= 0.0f || Player.Singleton.FreeTowers)
			{
				Tower NewTower = TowerTemplate.Instantiate<Tower>();
        		if (NewTower == null) return;

				NewTower.TotalCost += TowerData.Cost;
        		NewTower.Position = Position;
        		MainMap.Singleton.AddChild(NewTower);

				foreach(Node n in GetChildren())
				{
					if (n is ITowerComponent towerComp)
					{
						towerComp.TowerRemoved();
					}
				}
				PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerRemoved, this);
				PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerFinished, NewTower);
				QueueFree();
			}
		}
	}

	public void SetData(Data_Tower NewData)
	{
		TowerData = NewData;
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
	}
}
