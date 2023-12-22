using Godot;
using System;

public partial class C_HarvestArea : Node2D
{
	[Export] public int Range = 1;
	int SubstanceIncome;
	int FlowIncome;
	int BreathIncome;
	int EnergyIncome;

	C_GenerateResource SubstanceGenerate;
	C_GenerateResource FlowGenerate;
	C_GenerateResource BreathGenerate;
	C_GenerateResource EnergyGenerate;

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
					//GD.Print("C_HarvetArea._Ready: Harvesting tile " + x.ToString() + "," + y.ToString());
					if (HarvestedCurrency == ECurrencyType.Substance) { SubstanceIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Flow) { FlowIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Breath) { BreathIncome++; }
					else if (HarvestedCurrency == ECurrencyType.Energy) { EnergyIncome++; }
				}
			}

			if (SubstanceIncome > 0)
			{
                SubstanceGenerate = new() { AddedIncome = SubstanceIncome };
                AddResourceIncome(ECurrencyType.Substance, SubstanceGenerate);
			}
			if (FlowIncome > 0)
			{
                FlowGenerate = new() { AddedIncome = FlowIncome };
                AddResourceIncome(ECurrencyType.Flow, FlowGenerate);
			}
			if (BreathIncome > 0)
			{
                BreathGenerate = new() { AddedIncome = BreathIncome };
                AddResourceIncome(ECurrencyType.Breath, BreathGenerate);
			}
			if (EnergyIncome > 0)
			{
                EnergyGenerate = new() { AddedIncome = EnergyIncome };
                AddResourceIncome(ECurrencyType.Energy, EnergyGenerate);
			}
		}
	}

    static void AddResourceIncome(ECurrencyType Type, C_GenerateResource Gen)
	{
		Currency CurrencyPool = Player.GetCurrency(Type);
		CurrencyPool?.AddGenerator(Gen);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
