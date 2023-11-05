using Godot;
using System;

public partial class C_GenerateResource : Node
{
    [Export] public ECurrencyType CurrencyType;
    [Export] public int AddedIncome;
    [Export] public int AddFlatAmount;
    [Export] public int IncreaseMaximum;

    public override void _Ready()
    {
        Currency CurrencyPool = Player.GetCurrency(CurrencyType);
        if (CurrencyPool != null)
        {
            CurrencyPool.AddGenerator(this);
        }
    }

    public override void _ExitTree()
    {
        Currency CurrencyPool = Player.GetCurrency(CurrencyType);
        if (CurrencyPool != null)
        {
            CurrencyPool.RemoveGenerator(this);
        }
    }
}
