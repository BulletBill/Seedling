using Godot;
using System;

public partial class Tower : Sprite2D
{
	public static readonly String GroupName = "Tower";
	public Vector2I MapPosition = new Vector2I();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
