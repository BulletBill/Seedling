using Godot;
using System;

public partial class WaveTimerDisplay : Node
{
    public override void _EnterTree()
    {
        EnemyEvent.Register(EnemyEvent.SignalName.WaveTimerChanged, Callable.From((int r, int m) => UpdateTimer(r,m)));
        EnemyEvent.Register(EnemyEvent.SignalName.TimedWaveCountChanged, Callable.From((int n) => UpdateWave(n)));
    }

    public void UpdateTimer(int SecondsRemaining, int SecondsMax)
    {
        ProgressBar Bar = GetNodeOrNull<ProgressBar>("Progress");
        if (IsInstanceValid(Bar))
        {
            Bar.MaxValue = SecondsMax;
            Bar.Value = SecondsMax - SecondsRemaining;
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
