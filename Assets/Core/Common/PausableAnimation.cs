using Godot;
using System;

public partial class PausableAnimation : AnimationPlayer
{
	[Export] public float SpeedVariance = 0.0f;
	float Variance = 1.0f;

	public override void _Ready()
	{
		Variance = 1.0f + MathHelper.GetFloatInRange(-SpeedVariance, SpeedVariance);
	}

	public override void _Process(double delta)
	{
		SpeedScale = Level.GetSpeed() * Variance;
	}
}
