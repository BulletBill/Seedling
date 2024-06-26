using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public enum ECurrencyType
{
    Lifeforce,
    Substance,
    Flow,
    Breath,
    Energy,
    None,
}

public partial class Currency : Node2D
{
    [Export] public ECurrencyType CurrencyType = ECurrencyType.Lifeforce;
    [Export] public Texture2D IconLarge;
    [Export] public Texture2D IconSmall;
    [Export] public bool ShowMaximum;
    [Export] public bool ShowBar;
    public float Amount { get; protected set; }
    public float MaximumAmount { get; protected set; }
    public float Income { get; protected set; } // Per second
    public override void _EnterTree()
    {
        if (GetParentOrNull<Player>() is Player PlayerParent)
        {
            PlayerParent.Currencies.Add(this.CurrencyType, this);
        }

        PlayerEvent.RegisterResourceAdd(CurrencyType, Callable.From((float n) => AddAmount(n)));
        PlayerEvent.RegisterResourceAddMax(CurrencyType, Callable.From((float n) => AddMax(n)));
        PlayerEvent.RegisterResourceAddIncome(CurrencyType, Callable.From((float f) => AddIncome(f)));
    }

    public override void _Ready()
    {
        PlayerEvent.Broadcast(CurrencyType.ToString() + "Changed", Amount, MaximumAmount);
    }

    public void AddAmount(float InAmount)
    {
        if (InAmount == 0) return;

        float PrevAmount = Amount;
        Amount += InAmount;
        if (MaximumAmount > 0 && CurrencyType != ECurrencyType.Lifeforce)
        {
            Amount = Math.Clamp(Amount, 0, MaximumAmount);
        }

        if (PrevAmount != Amount)
        {
            BroadcastChange();
        }
    }

    public void AddMax(float InMax)
    {
        if (InMax == 0) return;
        MaximumAmount += InMax;

        if (MaximumAmount > 0 && CurrencyType != ECurrencyType.Lifeforce)
        {
            Amount = Math.Clamp(Amount, 0, MaximumAmount);
        }
        BroadcastChange();
    }

    public float GetSpace()
    {
        return MaximumAmount - Amount;
    }

    public void AddIncome(float InIncome)
    {
        if (InIncome == 0) return;
        
        Income += InIncome;
        PlayerEvent.Broadcast(CurrencyType.ToString() + "IncomeChanged", Income);
        PlayerEvent.Broadcast(PlayerEvent.SignalName.AnyResourceChanged);
    }

    public void TriggerIncome()
    {
        AddAmount(Income);
    }

    void BroadcastChange()
    {
        PlayerEvent.Broadcast(CurrencyType.ToString() + "Changed", Amount, MaximumAmount);
        PlayerEvent.Broadcast(PlayerEvent.SignalName.AnyResourceChanged);
    }
}
