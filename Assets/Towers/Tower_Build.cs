using Godot;
using System;

public partial class Tower_Build : Sprite2D, IHoverable
{
	[Export] public Data_Tower TowerData;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
