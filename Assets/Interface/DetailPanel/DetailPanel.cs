using Godot;
using System;
using System.ComponentModel;

public partial class DetailPanel : Node2D
{
	Sprite2D Icon;
	RichTextLabel Header;
	RichTextLabel Body;

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerHovered, Callable.From((Data_Tower T) => TowerHovered(T)));
		PlayerEvent.Register(PlayerEvent.SignalName.TowerExitHovered, Callable.From(() => TowerExitHovered()));
	}

	public override void _Ready()
	{
		Icon = GetNodeOrNull<Sprite2D>("Icon");
		Header = GetNodeOrNull<RichTextLabel>("Header");
		Body = GetNodeOrNull<RichTextLabel>("Body");
	}

	public void TowerHovered(Data_Tower HoveredTower)
	{
		if (HoveredTower == null)
		{
			if (Icon != null) { Icon.Texture = null; }
			if (Header != null) { Header.Text = ""; }
			if (Body != null) { Body.Text = ""; }
			return;
		}
		
		if (Icon != null) { Icon.Texture = HoveredTower.PlacementSprite; }
		if (Header != null) { Header.Text = HoveredTower.TowerName; }
		if (Body != null) { Body.Text = HoveredTower.Description; }
	}

	public void TowerExitHovered()
	{
	}
}
