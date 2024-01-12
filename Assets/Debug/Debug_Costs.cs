using Godot;
using System;

public partial class Debug_Costs : CheckButton
{
	public void ToggleCosts()
	{
		if (IsInstanceValid(Player.Singleton))
		{
			Player.Singleton.FreeTowers = !Player.Singleton.FreeTowers;
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AnyResourceChanged);
		}
	}
}
