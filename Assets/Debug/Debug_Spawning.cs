using Godot;
using System;

public partial class Debug_Spawning : CheckButton
{
	public void ToggleCosts()
	{
		if (IsInstanceValid(Player.Singleton))
		{
			EnemyController.GetSpawnerBrain().DisableSpawns = !EnemyController.GetSpawnerBrain().DisableSpawns;
		}
	}
}
