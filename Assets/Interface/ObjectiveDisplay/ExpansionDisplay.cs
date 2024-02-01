using Godot;
using System;

public partial class ExpansionDisplay : Node
{
    public override void _EnterTree()
    {
        EnemyEvent.Register(EnemyEvent.SignalName.PlayerExpansionChanged, Callable.From((int r, int m) => UpdateExpansion(r,m)));
        EnemyEvent.Register(EnemyEvent.SignalName.ExpansionWaveCountChanged, Callable.From((int n) => UpdateWave(n)));
    }

    public void UpdateExpansion(int ExpansionLevel, int ExpansionMax)
    {
        ProgressBar Bar = GetNodeOrNull<ProgressBar>("Progress");
        if (IsInstanceValid(Bar))
        {
            Bar.MaxValue = ExpansionMax;
            Bar.Value = ExpansionMax - ExpansionLevel;
        }
    }

    public void UpdateWave(int WaveCount)
    {
        RichTextLabel Label = GetNodeOrNull<RichTextLabel>("Wave");
        if (IsInstanceValid(Label))
        {
            Label.Text = WaveCount.ToString();
        }
    }
}
