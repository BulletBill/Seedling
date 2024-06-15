using Godot;
using System;

public partial class TowerComponent : Node
{
	public virtual void TowerReady() {}
	public virtual void TowerRemoved() {}
	public virtual void TowerUpdated() {}
}
