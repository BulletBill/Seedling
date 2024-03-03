using Godot;
using System;
using System.Security;

public partial class ObjectiveList : Node2D
{
	int NeedHearts = 0;
	int NeedLeaves = 0;
	bool Survived = false;
	RichTextLabel Text;

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.HeartCountUpdated, Callable.From((int i) => UpdateHeartCount(i)));
		PlayerEvent.Register(PlayerEvent.SignalName.SunleafCountUpdated, Callable.From((int i) => UpdateSunleafCount(i)));
		PlayerEvent.Register(PlayerEvent.SignalName.FinalWaveSurvived, Callable.From(() => UpdateSurvived(true)));
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = GetNodeOrNull<RichTextLabel>("Text");
		UpdateText();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void UpdateText()
	{
		if (!IsInstanceValid(Text)) return;

		Text.Text = "";
		if (NeedHearts > 0)
		{
			Text.Text += "Build " + NeedHearts.ToString() + " more Bloom Heart\n";
		}
		if (NeedLeaves > 0)
		{
			Text.Text += "Build " + NeedLeaves.ToString() + " more SunLeaf\n";
		}
		if (Survived == false)
		{
			Text.Text += "Survive the final wave";
		}
	}

	void UpdateHeartCount(int NewCount)
	{
		NeedHearts = NewCount;
		UpdateText();
	}

	void UpdateSunleafCount(int NewCount)
	{
		NeedLeaves = NewCount;
		UpdateText();
	}

	void UpdateSurvived(bool NewSurvived)
	{
		Survived = NewSurvived;
		UpdateText();
	}
}
