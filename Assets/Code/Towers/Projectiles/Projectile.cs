using Godot;
using System;

public partial class Projectile : Sprite2D
{
	public Vector2 Origin = new();
	public Enemy Target = new();
	public float RemainingTime = 1.0f;
	float StartTime = 1.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Assign(Vector2 StartPosition, Enemy TargetEnemy, float TravelTime)
	{
		Origin = StartPosition;
		GlobalPosition = StartPosition;

		Target = TargetEnemy;

		RemainingTime = TravelTime;
		StartTime = TravelTime;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (RemainingTime > 0.0f)
		{
			RemainingTime -= (float)delta;
			if (RemainingTime <= 0.0f)
			{
				QueueFree();
			}

			GlobalPosition = Origin.Lerp(Target.GlobalPosition, 1 - (RemainingTime / StartTime));
			LookAt(Target.GlobalPosition);
		}
	}
}
