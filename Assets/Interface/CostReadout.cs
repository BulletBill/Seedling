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
		Text = "";
		Text += "[center]";
		if (Cost.LifeForce != 0) { Text += TextHelpers.Icon("Lifeforce Small") + Cost.LifeForce.ToString() + " "; }
		if (Cost.Substance != 0) { Text += TextHelpers.Icon("Substance Small") + Cost.Substance.ToString() + " "; }
		if (Cost.Flow != 0) { Text += TextHelpers.Icon("Flow Small") + Cost.Flow.ToString() + " "; }
		if (Cost.Breath != 0) { Text += TextHelpers.Icon("Breath Small") + Cost.Breath.ToString() + " "; }
		if (Cost.Energy != 0) { Text += TextHelpers.Icon("Energy Small") + Cost.Energy.ToString() + " "; }
		Text += "[/center]";
	}
}
