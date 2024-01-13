using Godot;
using System;

public partial class C_HarvestArea : Node2D
{
	[Export] public int Range = 1;
	Vector2 ParentPositon;
	float IncomeTimer = 1.0f;
	int SubstanceIncome;
	int FlowIncome;
	int BreathIncome;
	int EnergyIncome;
	ProgressBar TimerBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GetParentOrNull<Tower>() != null)
		{
			for (int x = -1 * Range; x <= Range; x++)
			{
				for (int y = -1 * Range; y <= Range; y++)
				{
					ECurrencyType HarvestedCurrency = MainMap.HarvestTile(GetParentOrNull<Tower>().MapPosition + new Vector2I(x, y));
					if (HarvestedCurrency == ECurrencyType.Substance) { SubstanceIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Flow) { FlowIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Breath) { BreathIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Energy) { EnergyIncome++; }
					//GD.Print("C_HarvetArea._Ready: Harvesting tile " + x.ToString() + "," + y.ToString());
				}
			}
			ParentPositon = GetParent<Node2D>().GlobalPosition;
		}
		TimerBar = GetNodeOrNull<ProgressBar>("TimerBar");
		IncomeTimer = Player.IncomeTime;

		if (SubstanceIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, SubstanceIncome); }
		if (FlowIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, FlowIncome); }
		if (BreathIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, BreathIncome); }
		if (EnergyIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, EnergyIncome); }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Player.IncomeTime <= 0.0f) return;

		IncomeTimer -= (float)delta * Game.GetSpeed();
		if (IncomeTimer <= 0.0f)
		{
			IncomeTimer = Player.IncomeTime;
			if (SubstanceIncome > 0)
			{
				PlayerEvent.Broadcast(PlayerEvent.SignalName.AddSubstance, SubstanceIncome);
			}
			if (FlowIncome > 0)
			{
				PlayerEvent.Broadcast(PlayerEvent.SignalName.AddFlow, FlowIncome);
			}
			if (BreathIncome > 0)
			{
				PlayerEvent.Broadcast(PlayerEvent.SignalName.AddBreath, BreathIncome);
			}
			if (EnergyIncome > 0)
			{
				PlayerEvent.Broadcast(PlayerEvent.SignalName.AddEnergy, EnergyIncome);
			}
			Game.SpawnResourceCluster(ParentPositon, SubstanceIncome, FlowIncome, BreathIncome, EnergyIncome);
		}

		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Value = TimerBar.MaxValue - ((IncomeTimer / Player.IncomeTime) * 100.0f);
		}
	}
}
