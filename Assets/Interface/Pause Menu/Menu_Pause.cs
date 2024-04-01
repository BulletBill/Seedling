using Godot;
using System;

public partial class Menu_Pause : PopupMenu
{
    public override void _Ready()
	{
		base._Ready();

		MenuEvent.Register(MenuEvent.SignalName.OpenPauseMenu, Callable.From(() => Open()));
		Cursor.Register(Cursor.SignalName.AnyStateChanged, Callable.From(() => StateChanged()));
	}

	public override void Open()
	{
		base.Open();
	}

	public void StateChanged()
	{
		if (Cursor.GetCurrentState() != ECursorState.Menu_Pause)
		{
			Close();
		}
	}
}