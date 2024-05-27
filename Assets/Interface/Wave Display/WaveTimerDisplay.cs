using Godot;
using System;

public partial class WaveTimerDisplay : Node
{
    public override void _EnterTree()
    {
        EnemyEvent.Register(EnemyEvent.SignalName.WaveTimerChanged, Callable.From((int r, int m) => UpdateTimer(r,m)));
        EnemyEvent.Register(EnemyEvent.SignalName.TimedWaveCountChanged, Callable.From((int n) => UpdateWave(n)));
        EnemyEvent.Register(EnemyEvent.SignalName.StartFinalWave, Callable.From(() => StartFinalWave()));
    }

    public void UpdateTimer(int SecondsRemaining, int SecondsMax)
    {
        ProgressBar Bar = GetNodeOrNull<ProgressBar>("Progress");
        if (IsInstanceValid(Bar))
        {
            Bar.MaxValue = SecondsMax;
            Bar.Value = SecondsMax - SecondsRemaining;
        }
        RichTextLabel ClockLabel = GetNodeOrNull<RichTextLabel>("Clock");
        if (IsInstanceValid(ClockLabel))
        {
            ClockLabel.Text = MathHelper.GetTimeFromSeconds(SecondsRemaining);
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

    public void StartFinalWave()
    {
        ProgressBar Bar = GetNodeOrNull<ProgressBar>("Progress");
        if (IsInstanceValid(Bar))
        {
            Bar.Value = Bar.MaxValue;
        }

        AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (IsInstanceValid(Anim))
        {
            Anim.Play("Shimmer");
        }

        RichTextLabel Wave = GetNodeOrNull<RichTextLabel>("Wave");
        if (IsInstanceValid(Wave))
        {
            Wave.Text = "";
        }

        RichTextLabel Label = GetNodeOrNull<RichTextLabel>("Label");
        if (IsInstanceValid(Label))
        {
            Label.Text = "Final Wave";
        }
        RichTextLabel ClockLabel = GetNodeOrNull<RichTextLabel>("Clock");
        if (IsInstanceValid(ClockLabel))
        {
            ClockLabel.Visible = false;
        }
    }
}
