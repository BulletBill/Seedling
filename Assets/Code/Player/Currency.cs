using Godot;
using System;

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
    public int HeldAmount { get; protected set; }
    public int MaximumAmount { get; protected set; }
    public int Income { get; protected set; }

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

    public void AddAmount(int Amount)
    {
        int PrevAmount = HeldAmount;
        HeldAmount = Math.Clamp(HeldAmount + Amount, 0, MaximumAmount);

        if (PrevAmount != HeldAmount)
        {
            EmitSignal("OnCurrencyChanged", this);
        }
    }
}
