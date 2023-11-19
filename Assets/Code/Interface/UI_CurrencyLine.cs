using Godot;
using System;

public partial class UI_CurrencyLine : Node2D
{
	[Export] public ECurrencyType CurrencyType;
	[Export] public bool ShowMaximum = true;
	
	// TODO: Add UI color data store
	public readonly Color ExcessColor = Colors.Green;
	public readonly Color DeficitColor = Colors.Red;
	Sprite2D Icon;
	RichTextLabel Amount;
	RichTextLabel Income;

	public override void _Ready()
	{
		Icon = GetNodeOrNull<Sprite2D>("Icon");
		Amount = GetNodeOrNull<RichTextLabel>("Count");
		Income = GetNodeOrNull<RichTextLabel>("Income");

		Currency TargetCurrency = Player.GetCurrency(CurrencyType);
		if (TargetCurrency != null)
		{
			TargetCurrency.OnCurrencyChanged += OnCurrencyChanged;
			OnCurrencyChanged(TargetCurrency);
			if (Icon != null)
			{
				Icon.Texture = TargetCurrency.IconSmall;
			}
		}
	}

	public override void _Process(double delta)
	{
	}

	void OnCurrencyChanged(Currency UpdatedCurrency)
	{
		if (Amount != null)
		{
			int Value = UpdatedCurrency.HeldAmount;

			int MaxValue = UpdatedCurrency.MaximumAmount;
			Amount.Text = Value.ToString();
			if (MaxValue > 0 && ShowMaximum)
			{
				Amount.AppendText("/" + MaxValue.ToString());
				if (Value > MaxValue)
				{
					Amount.Text = TextHelpers.Colorize(Amount.Text, DeficitColor);
				}
			}
		}

		if (Income != null)
		{
			int Value = UpdatedCurrency.Income;
			if (Value == 0)
			{
				Income.Text = "";
			}
			else if (Value > 0)
			{
				Income.Text = TextHelpers.Colorize("+" + Value.ToString(), ExcessColor);
			}
			else if (Value < 0)
			{
				Income.Text = TextHelpers.Colorize("-" + Value.ToString(), DeficitColor);
			}
		}
	}
}
