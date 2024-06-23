using Godot;
using System;
using Godot.Collections;

public partial class C_HarvestArea : TowerComponent
{
	[Export] public int Range = 1;
	[Export] public float IncomePerTile = 0.2f;
	Vector2 ParentPositon;
	Vector2I MapPosition;
	Array<Vector2I> HarvestedPositions = new();
	float SubstanceIncome;
	float FlowIncome;
	float BreathIncome;
	float EnergyIncome;
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
	}

	// Called when the node enters the scene tree for the first time.
	public override void TowerReady()
	{
		if (GetParentOrNull<Tower>() != null)
		{
			ParentPositon = GetParent<Tower>().GlobalPosition;
			MapPosition = GetParent<Tower>().MapPosition;
		}

		ShowText(MainMap.IsOutlineActive());
		CalculateIncome();
	}

	public override void TowerRemoved()
	{
		RemoveSelf();
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
					if (HarvestedCurrency == ECurrencyType.Substance) { SubstanceIncome+=IncomePerTile; AddedSubstance+=IncomePerTile; }
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
			R_Income IncomeStruct = new(0, SubstanceIncome, FlowIncome, BreathIncome, EnergyIncome);
			IncomeText.SetIncome(IncomeStruct);
		}
	}

	void RemoveSelf()
	{
		if (SubstanceIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Substance, SubstanceIncome * -1); }
		if (FlowIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Flow, FlowIncome * -1); }
		if (BreathIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Breath, BreathIncome * -1); }
		if (EnergyIncome > 0) { PlayerEvent.BroadcastAddIncome(ECurrencyType.Energy, EnergyIncome * -1); }

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
