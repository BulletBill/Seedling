using Godot;
using System;

public partial class Debug_Spawning : CheckButton
{
	bool ButtonOn = false;
	public void ToggleCosts()
	{
		if (IsInstanceValid(Player.Singleton))
		{
			ButtonOn = !ButtonOn;
			EnemyEvent.Broadcast(EnemyEvent.SignalName.SetDisableSpawns, ButtonOn);
		}
	}
}
