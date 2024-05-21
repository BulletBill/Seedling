using Godot;
using System;

public partial class EnemyComponent : Node2D
{
	public virtual void OnDataSet(Data_Enemy NewData) {}

	public virtual void OnReady() {}

	public virtual void OnDeath() {}
}
