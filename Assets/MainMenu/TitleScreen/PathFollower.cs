using Godot;
using Microsoft.VisualBasic.FileIO;
using System;

public partial class PathFollower : PathFollow2D
{
	[Export] public float MinSpeed = 40.0f;
	[Export] public float MaxSpeed = 45.0f;
	public float Speed = 40.0f;

	public override void _Ready()
	{
		Speed = MathHelper.GetFloatInRange(MinSpeed, MaxSpeed);
	}

	public override void _Process(double delta)
	{
		Progress += Speed * (float)delta;
	}
}
