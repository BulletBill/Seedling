using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Player : Node2D
{
    public static Player Singleton;
    [Export] public int StartingLives = 12;
    public int Lives { get; protected set;}
    [Export] public float IncomeInterval;
    public float IncomeTimer { get; protected set; } = 0.1f;
    public Dictionary<ECurrencyType, Currency> Currencies = new();
    public static List<Tower> DefendTargets = new();

    // Signals
    [Signal] public delegate void ResourcesChangedEventHandler();
    [Signal] public delegate void GrassGrownEventHandler(int Count);
    [Signal] public delegate void LivesChangedEventHandler();

    public override void _EnterTree()
    {
        Player.Singleton = this;
    }

    public override void _Ready()
    {
        Lives = StartingLives;
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

    void TakeDamage_Internal(int Damage)
    {
        Lives -= Damage;
        EmitSignal("LivesChanged");
        if (Lives <= 0)
        {
            GetTree().Quit();
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

    public static bool CanAfford(R_Cost Cost)
    {
        if (Player.Singleton == null) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Lifeforce].Amount < Cost.LifeForce) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Substance].Amount < Cost.Substance) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Flow].Amount < Cost.Flow) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Breath].Amount < Cost.Breath) return false;
        if (Player.Singleton.Currencies[ECurrencyType.Energy].Amount < Cost.Energy) return false;

        return true;
    }

    public static bool Spend(R_Cost Cost)
    {
        if (Player.Singleton == null) return false;
        if (Player.CanAfford(Cost) == false) return false;

        Player.GetCurrency(ECurrencyType.Lifeforce).AddAmount(-1 * Cost.LifeForce);
        Player.GetCurrency(ECurrencyType.Substance).AddAmount(-1 * Cost.Substance);
        Player.GetCurrency(ECurrencyType.Flow).AddAmount(-1 * Cost.Flow);
        Player.GetCurrency(ECurrencyType.Breath).AddAmount(-1 * Cost.Breath);
        Player.GetCurrency(ECurrencyType.Energy).AddAmount(-1 * Cost.Energy);

        return true;
    }

    public static void TakeDamage(int Damage)
    {
        if (Player.Singleton == null) return;

        Player.Singleton.TakeDamage_Internal(Damage);
    }
}
