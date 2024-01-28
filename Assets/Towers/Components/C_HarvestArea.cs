using Godot;
using System;
using Godot.Collections;

public partial class C_HarvestArea : Node2D, ITowerComponent
{
	[Export] public int Range = 1;
	Vector2 ParentPositon;
	Vector2I MapPosition;
	Array<Vector2I> HarvestedPositions = new();
	float IncomeTimer = 1.0f;
	int SubstanceIncome;
	int FlowIncome;
	int BreathIncome;
	int EnergyIncome;
	ProgressBar TimerBar;
	public static readonly String HarvestGroup = "HarvestArea";

	// Called when the node enters the scene tree for the first time.
	public void TowerReady()
	{
		if (GetParentOrNull<Tower>() != null)
		{
			ParentPositon = GetParent<Tower>().GlobalPosition;
			MapPosition = GetParent<Tower>().MapPosition;
		}

		CalculateIncome();

		TimerBar = GetNodeOrNull<ProgressBar>("TimerBar");
		IncomeTimer = Player.IncomeTime;
	}

	public void TowerRemoved()
	{
		RemoveSelf();
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
			EffectsManager.SpawnResourceCluster(ParentPositon, SubstanceIncome, FlowIncome, BreathIncome, EnergyIncome);
		}

		if (IsInstanceValid(TimerBar))
		{
			TimerBar.Value = TimerBar.MaxValue - ((IncomeTimer / Player.IncomeTime) * 100.0f);
		}
	}

	void CalculateIncome()
	{
		int AddedSubstance = 0;
		int AddedFlow = 0;
		int AddedBreath = 0;
		int AddedEnergy = 0;
		for (int x = -1 * Range; x <= Range; x++)
		{
			for (int y = -1 * Range; y <= Range; y++)
			{
				Vector2I HarvestTile = MapPosition + new Vector2I(x, y);
				if (HarvestedPositions.Contains(HarvestTile)) continue;

				ECurrencyType HarvestedCurrency = MainMap.HarvestTile(HarvestTile);
				if (HarvestedCurrency != ECurrencyType.None)
				{
					HarvestedPositions.Add(HarvestTile);
					if (HarvestedCurrency == ECurrencyType.Substance) { SubstanceIncome++; AddedSubstance++; }
					else if (HarvestedCurrency == ECurrencyType.Flow) { FlowIncome++; AddedFlow++; }
					else if (HarvestedCurrency == ECurrencyType.Breath) { BreathIncome++; AddedBreath++; }
					else if (HarvestedCurrency == ECurrencyType.Energy) { EnergyIncome++; AddedEnergy++; }
				}
			}
		}

		if (AddedSubstance > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, AddedSubstance); }
		if (AddedFlow > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, AddedFlow); }
		if (AddedBreath > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, AddedBreath); }
		if (AddedEnergy > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, AddedEnergy); }

		CostReadout IncomeText = GetNodeOrNull<CostReadout>("Income");
		if (IsInstanceValid(IncomeText))
		{
			R_Cost IncomeStruct = new(0, SubstanceIncome, FlowIncome, BreathIncome, EnergyIncome);
			IncomeText.SetCosts(IncomeStruct);
		}
	}

	void RemoveSelf()
	{
		if (SubstanceIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, -SubstanceIncome); }
		if (FlowIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, -FlowIncome); }
		if (BreathIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, -BreathIncome); }
		if (EnergyIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, -EnergyIncome); }

		foreach (Vector2I clearTile in HarvestedPositions)
		{
			MainMap.UnHarvestTile(clearTile);
		}

		foreach(Node n in GetTree().GetNodesInGroup(C_HarvestArea.HarvestGroup))
		{
			if (n == this) continue;
			if (n is C_HarvestArea hArea)
			{
				hArea.CalculateIncome();
			}
		}
	}
}
