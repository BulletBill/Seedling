using Godot;
using System;

public partial class BuildTower : Node2D
{
	[Export] public PackedScene TowerToBecome = null;
	[Export] public double BuildTime = 1.0f;
	ProgressBar TimerBar;
	public double BuildTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BuildTimer = BuildTime;
		TimerBar = GetNodeOrNull<ProgressBar>("BuildBar");
		if (IsInstanceValid(TimerBar)) { TimerBar.MaxValue = (int)(BuildTime * 100); }
	}

	public void Assign(PackedScene NewTower, float NewBuildTime)
	{
		TowerToBecome = NewTower;
		BuildTime = NewBuildTime;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (BuildTimer > 0.0f)
		{
			BuildTimer -= delta;
			if (IsInstanceValid(TimerBar))
			{
				TimerBar.Value = TimerBar.MaxValue - (int)((BuildTimer / BuildTime) * TimerBar.MaxValue);
				//GD.Print("BuildTower._Process: Setting value to " + TimerBar.Value.ToString() + "/" + TimerBar.MaxValue.ToString());
			}
			
			if (BuildTimer <= 0.0f)
			{
				Node2D NewTower = TowerToBecome.Instantiate<Node2D>();
        		if (NewTower == null) return;
        		NewTower.Position = Position;
        		MainMap.Singleton.AddChild(NewTower);
				//QueueFree();
				Visible = false;
			}
		}
	}
}
