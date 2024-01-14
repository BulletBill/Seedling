using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public static readonly float SeekInterval = 1.0f;
	public static readonly String EnemyGroup = "Enemy";
	[Export] public int SpawnCost = 1;
	[Export] public int PlayerDamage = 1;
	[Export] public float Speed = 40.0f;
	[Export] public R_Cost Reward = new();
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
		Nav.TargetPosition = Player.DefendTargets[MathHelper.GetIntInRange(0, Player.DefendTargets.Count - 1)].GlobalPosition;
		HealthPool = GetNodeOrNull<C_HealthPool>("HealthPool");
		//MakePath();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Active) return;

		SeekTimer -= (float)delta * Game.GetSpeed();
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
		Velocity = (NextMove - GlobalPosition).Normalized() * (Speed * Game.GetSpeed());
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
		TargetReachable = Nav.IsTargetReachable();
		if (!TargetReachable)
		{
			GD.PrintErr("Enemy.MakePath: " + Name + " cannot reach the target!");
		}
	}

	public bool IsAlive()
	{
		if (HealthPool == null) return false;
		return HealthPool.IsAlive();
	}

	public int GetRemainingHealth()
	{
		if (HealthPool == null) return -1;

		return HealthPool.CurrentHealth;
	}

	public void Die()
	{
		// Reward player
		if (Reward.Substance > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddSubstance, Reward.Substance);
		}
		if (Reward.Flow > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddFlow, Reward.Flow);
		}
		if (Reward.Breath > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddBreath, Reward.Breath);
		}
		if (Reward.Energy > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddEnergy, Reward.Energy);
		}
		EffectsManager.SpawnResourceCluster(GlobalPosition, Reward.Substance, Reward.Flow, Reward.Breath, Reward.Energy);

		// Actually die
		QueueFree();
	}
}
