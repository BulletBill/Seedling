using Godot;
using System;

public partial class Popup_Victory : PopupMenu
{
	public override void _Ready()
	{
		base._Ready();

		MenuEvent.Register(MenuEvent.SignalName.OpenVictoryMenu, Callable.From(() => Open()));
	}

	public override void Open()
	{
		base.Open();

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("VictorySheen");
	}
}
