using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public static readonly float SeekInterval = 1.0f;
	public static readonly String EnemyGroup = "Enemy";
	[Export] public int SpawnCost = 1;
	[Export] public int PlayerDamage = 1;
	[Export] public float Speed = 40.0f;
	public bool Active = false;
	NavigationAgent2D Nav;
	float SeekTimer;
	bool TargetReachable;
	C_HealthPool HealthPool;

	// Targeting Metrics
	public float HealthPercent { get; protected set; } = 100.0f;
	public float DistanceToTarget { get; protected set; } = 0.0f;

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
		if (!Active) return;

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

		DistanceToTarget = Nav.DistanceToTarget();
		if (DistanceToTarget < MainMap.GetTileSize())
		{
			PlayerEvent.Bus.EmitSignal(PlayerEvent.SignalName.LoseLife, PlayerDamage);
			QueueFree();
		}
    }

	void MakePath()
	{
		if (Nav == null) return;
		if (Player.DefendTargets.Count <= 0) return;
		Nav.TargetPosition = Player.DefendTargets[0].GlobalPosition;
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

	public int GetRemainingHealth()
	{
		if (HealthPool == null) return -1;

		return HealthPool.CurrentHealth;
	}
}
