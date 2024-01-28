using Godot;
using System;
using System.Dynamic;
using System.Linq;

public partial class CostReadout : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public void SetCosts(R_Cost Cost)
	{
		bool FirstResult = true;
		Text = "";
		Text += "[center]";
		if (Cost.LifeForce != 0)
		{
			Text += TextHelpers.Icon("Lifeforce Small") + Cost.LifeForce.ToString();
			FirstResult = false;
		}
		if (Cost.Substance != 0)
		{
			if (!FirstResult) { Text += " "; }
			Text += TextHelpers.Icon("Substance Small") + Cost.Substance.ToString();
			FirstResult = false;
		}
		if (Cost.Flow != 0)
		{
			if (!FirstResult) { Text += " "; }
			Text += TextHelpers.Icon("Flow Small") + Cost.Flow.ToString();
			FirstResult = false;
		}
		if (Cost.Breath != 0)
		{
			if (!FirstResult) { Text += " "; }
			Text += TextHelpers.Icon("Breath Small") + Cost.Breath.ToString();
			FirstResult = false;
		}
		if (Cost.Energy != 0)
		{
			if (!FirstResult) { Text += " "; }
			Text += TextHelpers.Icon("Energy Small") + Cost.Energy.ToString();
		}
		Text += "[/center]";
	}
}
