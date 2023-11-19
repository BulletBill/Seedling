using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
    static Player Singleton;
    [Export] public float IncomeInterval;
    public float IncomeTimer { get; protected set; } = 0.1f;
    public Dictionary<ECurrencyType, Currency> Currencies = new();

    public override void _Ready()
    {
        Player.Singleton = this;
    }

    public override void _Process(double delta)
    {
        if (IncomeTimer >= 0.0f)
        {
            IncomeTimer -= (float)delta;

            if (IncomeTimer < 0.0f)
            {
                CurrencyTick();
                IncomeTimer += IncomeInterval;
            }
        }
    }

    public void CurrencyTick()
    {
        foreach(var c in Currencies)
        {
            c.Value.CurrencyTick();
        }
    }

    // Static functions
    public static Currency GetCurrency(ECurrencyType Type)
    {
        if (Player.Singleton == null) return null;
        if (Player.Singleton.Currencies.ContainsKey(Type) == false) return null;
        return Player.Singleton.Currencies[Type];
    }

    public static float GetIncomePercent()
    {
        if (Player.Singleton == null) return 0.0f;
        if (Player.Singleton.IncomeInterval == 0.0f) return 0.0f;

        return (1 - (Player.Singleton.IncomeTimer / Player.Singleton.IncomeInterval)) * 100.0f;
    }

    public static bool CanAfford(R_BuildTower BuildTower)
    {
        if (Player.Singleton == null) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Lifeforce].HeldAmount < BuildTower.LifeForceCost) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Substance].HeldAmount < BuildTower.SubstanceCost) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Flow].HeldAmount < BuildTower.FlowCost) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Breath].HeldAmount < BuildTower.BreathCost) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Energy].HeldAmount < BuildTower.EnergyCost) return false;

        return true;
    }
}
