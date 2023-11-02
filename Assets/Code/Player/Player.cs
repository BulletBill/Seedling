using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node2D
{
    public Dictionary<ECurrencyType, Currency> Currencies;

    public void CurrencyTick()
    {
        foreach(var c in Currencies)
        {
            c.Value.CurrencyTick();
        }
    }
}
