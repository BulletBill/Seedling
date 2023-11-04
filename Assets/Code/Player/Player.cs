using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
    [Export] public float IncomeInterval;
    float IncomeTimer = 0.1f;
    public Dictionary<ECurrencyType, Currency> Currencies = new Dictionary<ECurrencyType, Currency>();

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
}
