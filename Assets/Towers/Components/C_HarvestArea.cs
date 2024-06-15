using Godot;
using System;
using Godot.Collections;

public partial class C_HarvestArea : TowerComponent
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

	// Called when the node enters the scene tree for the first time.
	public override void TowerReady()
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

	public override void TowerRemoved()
	{
		RemoveSelf();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return; // Disable this feature. Income is handled by the player object.

		if (Player.IncomeTime <= 0.0f) return;

		IncomeTimer -= (float)delta * Level.GetSpeed();
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
		float AddedSubstance = 0;
		float AddedFlow = 0;
		float AddedBreath = 0;
		float AddedEnergy = 0;
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

		if (AddedSubstance > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, AddedSubstance / Player.IncomeTime); }
		if (AddedFlow > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, AddedFlow / Player.IncomeTime); }
		if (AddedBreath > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, AddedBreath / Player.IncomeTime); }
		if (AddedEnergy > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, AddedEnergy / Player.IncomeTime); }

		CostReadout IncomeText = GetNodeOrNull<CostReadout>("Income");
		if (IsInstanceValid(IncomeText))
		{
			R_Cost IncomeStruct = new(0, SubstanceIncome, FlowIncome, BreathIncome, EnergyIncome);
			IncomeText.SetCosts(IncomeStruct);
		}
	}

	void RemoveSelf()
	{
		if (SubstanceIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, -(SubstanceIncome / Player.IncomeTime)); }
		if (FlowIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, -(FlowIncome / Player.IncomeTime)); }
		if (BreathIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, -(BreathIncome / Player.IncomeTime)); }
		if (EnergyIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, -(EnergyIncome / Player.IncomeTime)); }

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
