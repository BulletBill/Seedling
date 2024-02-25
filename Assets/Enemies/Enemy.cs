using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public static readonly float SeekInterval = 1.0f;
	public static readonly String EnemyGroup = "Enemy";
	[Export] public Data_Enemy Data = new();
	//[Export] public int SpawnCost = 1;
	//[Export] public int PlayerDamage = 1;
	//[Export] public float Speed = 40.0f;
	//[Export] public R_Cost Reward = new();
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
		if (Player.DefendTargets.Count > 0)
		{
			Nav.TargetPosition = Player.DefendTargets[MathHelper.GetIntInRange(0, Player.DefendTargets.Count - 1)].GlobalPosition;
		}
		HealthPool = GetNodeOrNull<C_HealthPool>("HealthPool");
		if (HealthPool != null)
		{
			HealthPool.MinStartingHealth = Data.HealthRange.X;
			HealthPool.MaxStartingHealth = Data.HealthRange.Y;
			HealthPool.CalculateHealth();
		}
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
		Velocity = (NextMove - GlobalPosition).Normalized() * (Data.Speed * Game.GetSpeed());
		MoveAndSlide();

		DistanceToTarget = Nav.DistanceToTarget();
		if (DistanceToTarget < MainMap.GetTileSize())
		{
			PlayerEvent.Bus.EmitSignal(PlayerEvent.SignalName.LoseLife, Data.PlayerDamage);
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
		if (Data.Reward.Substance > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddSubstance, Data.Reward.Substance);
		}
		if (Data.Reward.Flow > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddFlow, Data.Reward.Flow);
		}
		if (Data.Reward.Breath > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddBreath, Data.Reward.Breath);
		}
		if (Data.Reward.Energy > 0)
		{
			PlayerEvent.Broadcast(PlayerEvent.SignalName.AddEnergy, Data.Reward.Energy);
		}
		EffectsManager.SpawnResourceCluster(GlobalPosition, Data.Reward.Substance, Data.Reward.Flow, Data.Reward.Breath, Data.Reward.Energy);

		// Actually die
		QueueFree();
	}
}
