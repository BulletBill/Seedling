using Godot;
using System;

public partial class Tower : Sprite2D, IHoverable
{
	[Export] public bool IsDefendTarget = false;
	[Export] public Data_Tower TowerData;
	public static readonly String GroupName = "Tower";
	public Vector2I MapPosition = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader };

		if (IsDefendTarget)
		{
			Player.DefendTargets.Add(this);
		}
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		Player.DefendTargets.Remove(this);
    }

	public void OnHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerHovered, TowerData);
	}

	public void ExitHovered()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerExitHovered);
	}
}