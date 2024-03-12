using Godot;
using System;

public partial class Menu_Pause : PopupMenu
{
    public override void _Ready()
	{
		base._Ready();

		MenuEvent.Register(MenuEvent.SignalName.OpenPauseMenu, Callable.From(() => Open()));
	}

	public override void Open()
	{
		base.Open();
	}
}