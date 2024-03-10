using Godot;
using System;

public partial class PopupMenu : Node2D
{
	[Export] public Vector2 OpenPosition = new();
	Vector2 ClosedPosition;


	public override void _Ready()
	{
		ClosedPosition = GlobalPosition;
	}

	public virtual void Open()
	{
		Visible = true;
		GlobalPosition = OpenPosition;
		Level.AddOpenMenu(this);
	}

	public virtual void Close()
	{
		Visible = false;
		GlobalPosition = ClosedPosition;
		Level.RemoveOpenMenu(this);
	}
}
