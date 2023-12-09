using Godot;
using System;
using System.Collections.Generic;

public enum ECurrencyType
{
    Lifeforce,
    Substance,
    Flow,
    Breath,
    Energy,
}

public partial class Currency : Node2D
{
    [Export] public ECurrencyType CurrencyType = ECurrencyType.Lifeforce;
    [Export] public Texture2D IconLarge;
    [Export] public Texture2D IconSmall;
    [Export] public bool ShowMaximum;
    [Export] public bool ShowBar;
    public int Amount { get; protected set; }
    public int MaximumAmount { get; protected set; }
    public int Income { get; protected set; }
    List<C_GenerateResource> Generators = new();

    [Signal] public delegate void OnCurrencyChangedEventHandler(Currency UpdatedCurrency);
    public override void _EnterTree()
    {
        if (GetParentOrNull<Player>() is Player PlayerParent)
        {
            PlayerParent.Currencies.Add(this.CurrencyType, this);
        }
    }

    public void CurrencyTick()
    {
        AddAmount(Income);
    }

    public void AddAmount(int InAmount)
    {
        int PrevAmount = Amount;
        Amount += InAmount;
        if (MaximumAmount > 0)
        {
            Amount = Math.Clamp(Amount, 0, MaximumAmount);
        }

        if (PrevAmount != Amount)
        {
            EmitSignal("OnCurrencyChanged", this);
        }
    }

    public void AddGenerator(C_GenerateResource NewGenerator)
    {
        if (Generators.Contains(NewGenerator)) return;

        Generators.Add(NewGenerator);
        MaximumAmount += NewGenerator.IncreaseMaximum;
        Income += NewGenerator.AddedIncome;
        Amount = Math.Clamp(Amount + NewGenerator.AddFlatAmount, 0, MaximumAmount);
        EmitSignal("OnCurrencyChanged", this);
    }

    public void RemoveGenerator(C_GenerateResource RemovedGenerator)
    {
        if (!Generators.Contains(RemovedGenerator)) return;

        Generators.Remove(RemovedGenerator);
        MaximumAmount -= RemovedGenerator.IncreaseMaximum;
        Income -= RemovedGenerator.AddedIncome;
        Amount = Math.Clamp(Amount - RemovedGenerator.AddFlatAmount, 0, MaximumAmount);
        EmitSignal("OnCurrencyChanged", this);
    }
}
