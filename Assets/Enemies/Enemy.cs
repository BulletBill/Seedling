using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	public static readonly float SeekInterval = 1.0f;
	public static readonly String EnemyGroup = "Enemy";
	public Data_Enemy Data { get; protected set; }
	public bool Active = false;
	float SpeedVariance = 1.0f;
	NavigationAgent2D Nav;
	float SeekTimer;
	bool TargetReachable;
	C_HealthPool HealthPool;
	Sprite2D Image;
	Sprite2D Shadow;
	List<EnemyComponent> Components = new();

	// Targeting Metrics
	public float HealthPercent { get; protected set; } = 100.0f;
	public float DistanceToTarget { get; protected set; } = 0.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is EnemyComponent childComponent)
			{
				Components.Add(childComponent);
				childComponent.OnDataSet(Data);
				childComponent.OnReady();
			}
		}
		
		Nav = GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D");
		if (Player.DefendTargets.Count > 0)
		{
			Nav.TargetPosition = Player.DefendTargets[MathHelper.GetIntInRange(0, Player.DefendTargets.Count - 1)].GlobalPosition;
		}
		HealthPool = GetNodeOrNull<C_HealthPool>("HealthPool");
		Image = GetNodeOrNull<Sprite2D>("Image");
		Shadow = GetNodeOrNull<Sprite2D>("Shadow");
		SpeedVariance = MathHelper.GetFloatInRange(0.95f, 1.05f);
	}

	public void SetData(Data_Enemy NewData)
	{
		Data = NewData;
		foreach (EnemyComponent childComponent in Components)
		{
			childComponent.OnDataSet(Data);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Active) return;

		SeekTimer -= (float)delta * Level.GetSpeed();
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
		Velocity = (NextMove - GlobalPosition).Normalized() * (Data.Speed * Level.GetSpeed() * SpeedVariance);
		Image?.LookAt(NextMove);
		if (Shadow != null) { Shadow.Rotation = Image.Rotation; }
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
			Game.LogError(LogCategory.Enemy, "Enemy.MakePath: " + Name + " cannot reach the target!");
			QueueFree();
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
		foreach (EnemyComponent childComponent in Components)
		{
			childComponent.OnDeath();
		}

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
		EnemyEvent.Broadcast(EnemyEvent.SignalName.EnemyDefeated, this);

		// Actually die
		QueueFree();
	}
}
