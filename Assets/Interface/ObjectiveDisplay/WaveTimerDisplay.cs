using Godot;
using System;

public partial class WaveTimerDisplay : Node
{
    SpawnerBrain Spawner;
    RichTextLabel Label;
    public override void _Ready()
    {
        Spawner = EnemyController.GetSpawnerBrain();
        Label = GetNodeOrNull<RichTextLabel>("Time");
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(Spawner) && IsInstanceValid(Label))
        {
            Label.Text = MathHelper.GetTimeFromSeconds(Spawner.BigWaveTimer);
        }
    }
}
