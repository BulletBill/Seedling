using Godot;
using System;

public partial class Tower_Build : Node2D, IHoverable
{
	public Data_Tower TowerData;
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
				Tower NewTower = GD.Load<PackedScene>("res://Assets/Towers/Tower_Template.tscn").InstantiateOrNull<Tower>();
        		if (NewTower == null)
				{
					Game.LogError(LogCategory.Tower, "Failed to create tower template!");
					return;
				}
				NewTower.SetData(TowerData);

				NewTower.TotalCost += TowerData.Cost;
        		NewTower.Position = Position;
        		MainMap.Singleton.AddChild(NewTower);

				foreach(Node n in GetChildren())
				{
					if (n is TowerComponent towerComp)
					{
						towerComp.TowerRemoved();
					}
				}
				
				PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerFinished, NewTower);
				QueueFree();
			}
		}
	}

	public void SetData(Data_Tower NewData)
	{
		if (NewData == null) return;

		TowerData = NewData;
		Name = TowerData.DisplayName + "_Build";

		Sprite2D Image = GetNodeOrNull<Sprite2D>("Image");
		Sprite2D Shadow = GetNodeOrNull<Sprite2D>("Shadow");

		if (IsInstanceValid(Image))
		{
			Image.Texture = TowerData.SpriteSheet;
		}
		if (IsInstanceValid(Shadow))
		{
			Shadow.Texture = TowerData.SpriteSheet;
		}
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
