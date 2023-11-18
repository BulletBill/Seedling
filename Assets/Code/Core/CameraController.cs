using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] public float MinimumZoom { get; protected set; } = 1.0f;
	[Export] public float MaximumZoom { get; protected set; } = 2.0f;
	[Export] public float PanSpeed { get; protected set; } = 250.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
