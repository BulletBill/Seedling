using Godot;
using System;

public partial class C_HealthPool : Node
{
    [Export] public int StartingHealth = 10;
    public int CurrentHealth;

    public override void _Ready()
    {
        CurrentHealth = StartingHealth;
    }

    public void TakeDamage(int IntDamage)
    {

    }
}
