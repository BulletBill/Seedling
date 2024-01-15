using Godot;
using System;

public partial class PausableAnimation : AnimationPlayer
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SpeedScale = Game.GetSpeed();
	}
}
