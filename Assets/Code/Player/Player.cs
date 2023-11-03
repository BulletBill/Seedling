using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
    public Dictionary<ECurrencyType, Currency> Currencies = new Dictionary<ECurrencyType, Currency>();

    public void CurrencyTick()
    {
        foreach(var c in Currencies)
        {
            c.Value.CurrencyTick();
        }
    }
}
