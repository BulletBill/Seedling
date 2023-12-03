using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 40.0f;
	NavigationAgent2D Nav;
	public static readonly float SeekInterval = 1.0f;
	float SeekTimer;
	bool TargetReachable;
	C_HealthPool HealthPool;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Nav = GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D");
		HealthPool = GetNodeOrNull<C_HealthPool>("HealthPool");
		//MakePath();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SeekTimer -= (float)delta;
		if (SeekTimer <= 0.0f)
		{
			MakePath();
			SeekTimer = SeekInterval;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Nav == null) return;
		if (!TargetReachable) return;
        Vector2 NextMove = Nav.GetNextPathPosition();
		Velocity = (NextMove - GlobalPosition).Normalized() * Speed;
		MoveAndSlide();
    }

	void MakePath()
	{
		if (Nav == null) return;
		if (Game.DefendTargets.Count <= 0) return;
		Nav.TargetPosition = Game.DefendTargets[0].GlobalPosition;
		TargetReachable = Nav.IsTargetReachable();
		if (!TargetReachable)
		{
			GD.PrintErr("Enemy.MakePath: " + Name + " cannot reach the target!");
		}
	}

	void TakeDamage(int InDamage)
	{
		if (HealthPool == null) return;
		HealthPool.TakeDamage(InDamage);
	}
}
