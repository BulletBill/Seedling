using Godot;
using System;

public partial class C_PassiveIncome : Node, ITowerComponent
{
    [Export] public float IncomeInterval = 5.0f;
    [Export] public R_Cost IncomeAmount = new();
    Vector2 ParentPositon;
	Vector2I MapPosition;
	float IncomeTimer = 1.0f;
	float SubstanceIncome;
	float FlowIncome;
	float BreathIncome;
	float EnergyIncome;
	ProgressBar TimerBar;

    public override void _EnterTree()
    {
        MainMap.Register(MainMap.SignalName.GridVisibleChanged, Callable.From((bool b) => ShowText(b)));
		ShowText(MainMap.IsOutlineActive());
    }

    void ShowText(bool NewShow)
	{
		RichTextLabel IncomeNode = GetNodeOrNull<RichTextLabel>("Income");
		if (IncomeNode != null)
		{
			IncomeNode.Visible = NewShow;
		}

		ProgressBar TimerBar = GetNodeOrNull<ProgressBar>("TimerBar");
		if (TimerBar != null)
		{
			TimerBar.Visible = NewShow;
		}
	}

    void CalculateIncome()
    {

    }

    // Tower Component interface
    public void TowerReady()
    {
    }

    public void TowerRemoved()
    {
    }

    public void TowerUpdated()
    {
    }
}
