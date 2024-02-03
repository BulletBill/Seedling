using Godot;
using System;
using System.Runtime.InteropServices;

public partial class LifeCounter : Node
{
    public override void _EnterTree()
    {
        PlayerEvent.Register(PlayerEvent.SignalName.LivesChanged, Callable.From((int n) => Update(n)));
    }
    public override void _Ready()
    {
    }

    void Update(int NewLives)
    {
        ProgressBar LifeBar = GetNodeOrNull<ProgressBar>("Lives Bar");
        if (LifeBar != null)
        {
            LifeBar.MaxValue = Player.Singleton.StartingLives;
            LifeBar.Value = NewLives;
        }

        RichTextLabel Label = GetNodeOrNull<RichTextLabel>("Count");
        if (Label != null)
        {
            Label.Text = NewLives.ToString();
        }
    }
}
