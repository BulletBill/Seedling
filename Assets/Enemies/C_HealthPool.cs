using Godot;
using System;

public partial class C_HealthPool : Node2D
{
    [Export] public int MinStartingHealth = 10;
    [Export] public int MaxStartingHealth = 12;
    public int StartingHealth;
    public int CurrentHealth;
    public int PendingDamage;
    ProgressBar HealthVisual;

    public override void _Ready()
    {
        StartingHealth = Game.GetIntInRange(MinStartingHealth, MaxStartingHealth);
        CurrentHealth = StartingHealth;
        HealthVisual = GetNodeOrNull<ProgressBar>("HealthBar");
        if (HealthVisual != null)
        {
            HealthVisual.MaxValue = StartingHealth;
        }
    }

    public void TakeDamage(int InDamage)
    {
        PendingDamage += InDamage;
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
