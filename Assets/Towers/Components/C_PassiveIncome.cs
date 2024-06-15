using Godot;
using System;

public partial class C_PassiveIncome : TowerComponent
{ 
    [Export] public R_Income IncomeAmount = new();
    Vector2 ParentPositon;
	Vector2I MapPosition;
	float IncomeTimer = 1.0f;
	float SubstanceIncome;
	float FlowIncome;
	float BreathIncome;
	float EnergyIncome;
	CostReadout IncomeDisplay;

    public override void _EnterTree()
    {
        IncomeDisplay = GetNodeOrNull<CostReadout>("IncomeDisplay");
        MainMap.Register(MainMap.SignalName.GridVisibleChanged, Callable.From((bool b) => ShowText(b)));
		ShowText(MainMap.IsOutlineActive());
    }

    void ShowText(bool NewShow)
	{
		CostReadout IncomeDisplay = GetNodeOrNull<CostReadout>("IncomeDisplay");
		if (IncomeDisplay != null)
		{
            IncomeDisplay.SetIncome(IncomeAmount);
			IncomeDisplay.Visible = NewShow;
		}
	}

    // Tower Component interface
    public override void TowerReady()
    {
        PlayerEvent.BroadcastAddIncome(ECurrencyType.Lifeforce, IncomeAmount.LifeForce);
        PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, IncomeAmount.Substance);
        PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, IncomeAmount.Flow);
        PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, IncomeAmount.Breath);
        PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, IncomeAmount.Energy);
    }
}
