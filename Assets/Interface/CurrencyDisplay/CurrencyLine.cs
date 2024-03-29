using Godot;
using System;

public partial class CurrencyLine : Node2D
{
	[Export] public ECurrencyType CurrencyType;
	[Export] public bool ShowMaximum = true;
	
	// TODO: Add UI color data store
	public readonly Color ExcessColor = Colors.Green;
	public readonly Color DeficitColor = Colors.Red;
	Sprite2D Icon;
	RichTextLabel Amount;
	RichTextLabel Income;

    public override void _EnterTree()
    {
		PlayerEvent.RegisterResourceChanged(CurrencyType, Callable.From((int n, int m) => CurrencyUpdate(n,m)));
		PlayerEvent.RegisterResourceIncome(CurrencyType, Callable.From((int n) => IncomeUpdate(n)));
    }

	public override void _Ready()
	{
		Icon = GetNodeOrNull<Sprite2D>("Icon");
		Amount = GetNodeOrNull<RichTextLabel>("Count");
		Income = GetNodeOrNull<RichTextLabel>("Income");

		CurrencyUpdate(Player.GetCurrentAmount(CurrencyType), Player.GetCurrentMax(CurrencyType));
		IncomeUpdate(Player.GetCurrentIncome(CurrencyType));
	}

	void IncomeUpdate(int NewIncome)
	{
		if (Income != null)
		{
			if (NewIncome == 0)
			{
				Income.Text = "";
			}
			else if (NewIncome > 0)
			{
				Income.Text = TextHelpers.Colorize("+" + NewIncome.ToString(), ExcessColor);
			}
			else if (NewIncome < 0)
			{
				Income.Text = TextHelpers.Colorize("-" + NewIncome.ToString(), DeficitColor);
			}
		}
	}
	void CurrencyUpdate(int NewAmount, int NewMax)
	{
		if (Amount != null)
		{
			Amount.Text = NewAmount.ToString();
			if (NewMax > 0 && ShowMaximum)
			{
				Amount.Text += "/" + NewMax.ToString();
			}

			if (NewMax > 0 && NewAmount >= NewMax)
			{
				Amount.Text = TextHelpers.Colorize(Amount.Text, ExcessColor);
			}
		}
	}
}
