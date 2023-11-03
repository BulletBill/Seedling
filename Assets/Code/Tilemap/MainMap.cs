using Godot;
using GodotPlugins.Game;
using System;

public partial class MainMap : TileMap
{
	public static MainMap Singleton;

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		MainMap.Singleton = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
