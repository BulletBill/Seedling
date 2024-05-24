using Godot;
using System;
using System.Buffers;
using System.IO;

public enum ETargetPriority
{
    ClosestToShooter,
    ClosestToFinish,
    HighestHealth,
    LowestHealth,
}

public partial class C_Attack : Node2D, ITowerComponent
{
    public static readonly String AttackTowerGroupName = "Attacker";
    [Export] public PackedScene FiredProjectile = null;
    [Export]public float DamageDelay; // Affects projectile speed
    public int MinDamage = 1;
    public int MaxDamage = 1;
    public float AttackDelay = 1.0f;
    public float Range = 100.0f;
    public float AreaOfEffect = 0.0f;
    [Export] public ETargetPriority TargetPriority;
    [Export] bool CanChangeTarget = false;
    Tower ParentTower = null;

    float AttackTimer;
    Enemy CurrentTarget;

    float DamageTimer = 0.0f;
    int PendingDamage = 0;
    Enemy PendingDamageTaker;

    public void TowerReady()
    {
        UpdateFromParentData();
    }

    public void TowerRemoved()
    {

    }

    public void TowerUpdated()
    {
        UpdateFromParentData();
    }

    public void UpdateFromParentData()
    {
        ParentTower = GetParentOrNull<Tower>();
        if (IsInstanceValid(ParentTower))
        {
            if (ParentTower.TowerData != null)
            {
                MinDamage = ParentTower.TowerData.MinDamage;
                MaxDamage = ParentTower.TowerData.MaxDamage;
                AttackDelay = ParentTower.TowerData.FireDelay;
                Range = ParentTower.TowerData.Range;
                AreaOfEffect = ParentTower.TowerData.AreaOfEffect;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (AttackTimer > 0 && IsInstanceValid(ParentTower) && !ParentTower.Upgrading)
        {
            AttackTimer -= (float)delta * Level.GetSpeed();
            if (AttackTimer <= 0.0f && IsInstanceValid(CurrentTarget))
            {
                Fire();
            }
        }

        if (DamageTimer > 0 && IsInstanceValid(PendingDamageTaker))
        {
            DamageTimer -= (float)delta * Level.GetSpeed();
            if (DamageTimer < 0.0f)
            {
                C_HealthPool TargetHealth = PendingDamageTaker.GetNodeOrNull<C_HealthPool>("HealthPool");
                TargetHealth.RealizeDamage(PendingDamage);
                if (AreaOfEffect > 1) { HitArea(); }

                PendingDamageTaker = null;
            }
        }
        else
        {
            PendingDamage = 0;
        }
    }

    public void SeekTarget()
    {
        if (IsInstanceValid(ParentTower) && ParentTower.Upgrading)
        {
            return;
        }

        if (IsInstanceValid(CurrentTarget) && !CanChangeTarget)
        {
            float Distance = GlobalPosition.DistanceTo(CurrentTarget.GlobalPosition);

            if (Distance > (Range * 2.0f)) 
            {
                CurrentTarget = null;
            }
            else
            {
                return;
            }
        }

        float BestScore = 0.0f;
        Enemy NewTarget = null;
        foreach(Node EnemyNode in GetTree().GetNodesInGroup(Enemy.EnemyGroup))
        {
            if (EnemyNode is not Enemy TestEnemy) continue;
            float Distance = GlobalPosition.DistanceTo(TestEnemy.GlobalPosition);

            if (Distance > (Range * 2.0f)) continue;
            if (!TestEnemy.IsAlive()) continue;

            float Score = 0.0f;
            if (TargetPriority == ETargetPriority.ClosestToShooter)
            {
                Score = 100000 - Distance;
            }
            else if (TargetPriority == ETargetPriority.ClosestToFinish)
            {
                Score = 100000 - TestEnemy.DistanceToTarget;
            }
            else if (TargetPriority == ETargetPriority.HighestHealth)
            {
                Score = TestEnemy.GetRemainingHealth();
            }
            else if (TargetPriority == ETargetPriority.LowestHealth)
            {
                Score = TestEnemy.GetRemainingHealth() * -1;
            }

            if (Score > BestScore)
            {
                NewTarget = TestEnemy;
                BestScore = Score;
            }
        }

        if (IsInstanceValid(NewTarget) && NewTarget != CurrentTarget)
        {
            Game.Log(LogCategory.Tower, Name + ": new target is " + NewTarget.Name);
            CurrentTarget = NewTarget;
        }

        if (IsInstanceValid(CurrentTarget) && AttackTimer <= 0.0f)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (!IsInstanceValid(CurrentTarget)) return;
        if (PendingDamage > 0)
        {
            Game.LogError(LogCategory.Tower, "Trying to fire before pending damage has been resolved!");
            return;
        }
        AttackTimer = AttackDelay;
        DamageTimer = DamageDelay * (GlobalPosition.DistanceTo(CurrentTarget.GlobalPosition) / Range);
        PendingDamageTaker = CurrentTarget;
        PendingDamage = MathHelper.GetIntInRange(MinDamage, MaxDamage);

        C_HealthPool TargetHealth = PendingDamageTaker.GetNodeOrNull<C_HealthPool>("HealthPool");
        PendingDamage = TargetHealth.TakeDamage(PendingDamage);

        if (FiredProjectile != null)
        {
            Projectile NewProjectile = FiredProjectile.InstantiateOrNull<Projectile>();
            if(NewProjectile != null)
            {
                AddChild(NewProjectile);
                NewProjectile.Assign(GlobalPosition, CurrentTarget, DamageTimer);
            }
        }

        AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("../Animator");
        if (Anim != null)
        {
            Anim.SpeedScale = AttackDelay > 0.0f ? 1.0f / AttackDelay : 1.0f;
            Anim.Play("Attack");
        }
    }

    void HitArea()
    {
        if (PendingDamageTaker == null) return;

        foreach(Node EnemyNode in GetTree().GetNodesInGroup(Enemy.EnemyGroup))
        {
            if (EnemyNode is not Enemy AreaEnemy) continue;
            if (AreaEnemy == PendingDamageTaker) continue;

            float Distance = PendingDamageTaker.GlobalPosition.DistanceTo(AreaEnemy.GlobalPosition);
            if (Distance > AreaOfEffect) continue;

            C_HealthPool AreaHealth = AreaEnemy.GetNodeOrNull<C_HealthPool>("HealthPool");
            if (AreaHealth != null)
            {
                AreaHealth.TakeDamageImmediate(MathHelper.GetIntInRange(MinDamage, MaxDamage));
            }
        }
    }

    public override void _Draw()
    {
        if (MainMap.IsOutlineActive())
        {
            //DrawArc(Position, Range, 0.0f, 360.0f, 36, Colors.OrangeRed);
        }
    }
}
