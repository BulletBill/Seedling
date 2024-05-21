using Godot;
using System;

public partial class C_SplitOnDeath : EnemyComponent
{
	[Export] public Data_Enemy SpawnData;
	[Export] public int SpawnCount = 4;
	public static readonly float SpawnDistance = 14;
	
	// Called when the node enters the scene tree for the first time.
	public override void OnReady() {}

    public override void OnDataSet(Data_Enemy NewData) {}

    public override void OnDeath()
    {
		Enemy ParentEnemy = GetParentOrNull<Enemy>();
		if (IsInstanceValid(ParentEnemy) == false) return;

        for (int i = 0; i < SpawnCount; i++)
		{
			Enemy newEnemy = SpawnData.Spawn();
			MainMap.Singleton.AddChild(newEnemy);
			newEnemy.GlobalPosition = ParentEnemy.GlobalPosition;
			MathHelper.PositionOffset(newEnemy.GlobalPosition, MathHelper.GetFloatInRange(0, (float)Math.PI * 2.0f), SpawnDistance);
			newEnemy.Active = true;
		}
    }
}
