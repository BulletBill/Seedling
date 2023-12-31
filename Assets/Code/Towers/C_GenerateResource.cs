using Godot;
using System;

public partial class C_GenerateResource : Node
{
    [Export] public ECurrencyType CurrencyType;
    [Export] public int AddedIncome;
    [Export] public int AddFlatAmount;
    [Export] public int IncreaseMaximum;
    public float IncomeTime;

    public override void _Ready()
    {
        if (AddFlatAmount != 0)
        {
            PlayerEvent.BroadcastAddResource(CurrencyType, AddFlatAmount);
        }
        if (IncreaseMaximum != 0)
        {
            PlayerEvent.Broadcast("Add" + CurrencyType.ToString() + "Max", IncreaseMaximum);
        }
    }

    public override void _ExitTree()
    {
        if (IncreaseMaximum != 0)
        {
            PlayerEvent.Broadcast("Add" + CurrencyType.ToString() + "Max", IncreaseMaximum * -1);
        }
    }
}
