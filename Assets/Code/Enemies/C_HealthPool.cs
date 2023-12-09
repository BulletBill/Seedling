using Godot;
using System;

public partial class C_HealthPool : Node2D
{
    [Export] public int StartingHealth = 10;
    public int CurrentHealth;
    ProgressBar HealthVisual;

    public override void _Ready()
    {
        CurrentHealth = StartingHealth;
        HealthVisual = GetNodeOrNull<ProgressBar>("HealthBar");
        if (HealthVisual != null)
        {
            HealthVisual.MaxValue = StartingHealth;
        }
    }

    public void TakeDamage(int InDamage)
    {
        CurrentHealth -= InDamage;
        if (HealthVisual != null && StartingHealth > 0)
        {
            HealthVisual.Value = CurrentHealth;
            HealthVisual.Visible = HealthVisual.Value < HealthVisual.MaxValue;
        }

        if (CurrentHealth <= 0) Die();
    }

    void Die()
    {
        GetParent().QueueFree();
    }
}
