using Godot;
using System;

public partial class BuildButton : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HoverArea hoverArea = GetNodeOrNull<HoverArea>("HoverArea");
		if (hoverArea != null)
		{
			hoverArea.Clicked += OnClick;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnClick()
	{
		Visible = !Visible;
	}
}
