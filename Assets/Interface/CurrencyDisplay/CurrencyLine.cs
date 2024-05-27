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
		PlayerEvent.RegisterResourceChanged(CurrencyType, Callable.From((float n, float m) => CurrencyUpdate(n,m)));
		PlayerEvent.RegisterResourceIncome(CurrencyType, Callable.From((float f) => IncomeUpdate(f)));
    }

	public override void _Ready()
	{
		Icon = GetNodeOrNull<Sprite2D>("Icon");
		Amount = GetNodeOrNull<RichTextLabel>("Count");
		Income = GetNodeOrNull<RichTextLabel>("Income");

		CurrencyUpdate(Player.GetCurrentAmount(CurrencyType), Player.GetCurrentMax(CurrencyType));
		IncomeUpdate(Player.GetCurrentIncome(CurrencyType));
	}

	void IncomeUpdate(float NewIncome)
	{
		if (Income != null)
		{
			if (NewIncome == 0)
			{
				Income.Text = "";
			}
			else if (NewIncome > 0)
			{
				Income.Text = TextHelpers.Colorize("+" + NewIncome.ToString("F1"), ExcessColor);
			}
			else if (NewIncome < 0)
			{
				Income.Text = TextHelpers.Colorize("-" + NewIncome.ToString("F1"), DeficitColor);
			}
		}
	}
	void CurrencyUpdate(float NewAmount, float NewMax)
	{
		int DisplayedAmount = Mathf.FloorToInt(NewAmount);
		int DisplayedMax = Mathf.FloorToInt(NewMax);
		if (Amount != null)
		{
			Amount.Text = DisplayedAmount.ToString();
			if (DisplayedMax > 0 && ShowMaximum)
			{
				Amount.Text += "/" + NewMax.ToString();
			}

			if (DisplayedMax > 0 && DisplayedAmount >= DisplayedMax)
			{
				if (CurrencyType == ECurrencyType.Lifeforce)
				{
					Amount.Text = TextHelpers.Colorize(Amount.Text, DeficitColor);
				}
				else
				{
					Amount.Text = TextHelpers.Colorize(Amount.Text, ExcessColor);
				}
			}
		}
	}
}
