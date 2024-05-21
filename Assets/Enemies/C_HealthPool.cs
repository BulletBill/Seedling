using Godot;
using System;
using System.ComponentModel;

public partial class C_HealthPool : EnemyComponent
{
    [Export] public int MinStartingHealth = 10;
    [Export] public int MaxStartingHealth = 12;
    [Export] public int Armor = 0;
    public int StartingHealth;
    public int CurrentHealth;
    public int PendingDamage;
    ProgressBar HealthVisual;

    public override void OnDataSet(Data_Enemy NewData)
    {
		if (NewData == null) return;
		
		MinStartingHealth = NewData.HealthRange.X;
		MaxStartingHealth = NewData.HealthRange.Y;
		CalculateHealth();
    }

    public void CalculateHealth()
    {
        StartingHealth = MathHelper.GetIntInRange(MinStartingHealth, MaxStartingHealth);
        CurrentHealth = StartingHealth;
        HealthVisual = GetNodeOrNull<ProgressBar>("HealthBar");
        if (HealthVisual != null)
        {
            HealthVisual.MaxValue = StartingHealth;
        }
    }

    public int TakeDamage(int InDamage)
    {
        int DamageTaken = Math.Clamp(InDamage - Armor, 1, InDamage);
        PendingDamage += DamageTaken;
        return DamageTaken;
    }

    public void TakeDamageImmediate(int InDamage)
    {
        int DamageTaken = TakeDamage(InDamage);
        RealizeDamage(DamageTaken);
    }

    public void RealizeDamage(int RealizedDamage)
    {
        PendingDamage -= RealizedDamage;
        CurrentHealth -= RealizedDamage;

        if (HealthVisual != null && StartingHealth > 0)
        {
            HealthVisual.Value = CurrentHealth;
            HealthVisual.Visible = HealthVisual.Value < HealthVisual.MaxValue;
        }

        if (CurrentHealth <= 0) Die();
    }

    public bool IsAlive()
    {
        return CurrentHealth > PendingDamage;
    }

    void Die()
    {
        GetParentOrNull<Enemy>()?.Die();
    }
}
