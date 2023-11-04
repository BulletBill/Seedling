using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
    static Player Singleton;
    [Export] public float IncomeInterval;
    float IncomeTimer = 0.1f;
    public Dictionary<ECurrencyType, Currency> Currencies = new();

    public override void _Ready()
    {
        Player.Singleton = this;
    }

    public override void _Process(double delta)
    {
        
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
}
