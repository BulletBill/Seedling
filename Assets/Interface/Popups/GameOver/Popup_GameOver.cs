using Godot;
using System;

public partial class Popup_GameOver : PopupMenu
{
	public override void _Ready()
	{
		base._Ready();

		MenuEvent.Register(MenuEvent.SignalName.OpenGameOverMenu, Callable.From(() => Open()));
	}

	public override void Open()
	{
		base.Open();

		AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Anim?.Play("DefeatWiggle");
	}
}
