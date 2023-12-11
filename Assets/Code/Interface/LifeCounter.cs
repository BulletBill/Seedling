using Godot;
using System;

public partial class LifeCounter : Node
{
    public override void _Ready()
    {
        Player.Singleton.LivesChanged += Update;
        Update();
    }

    void Update()
    {
        ProgressBar LifeBar = GetNodeOrNull<ProgressBar>("Lives Bar");
        if (LifeBar != null)
        {
            LifeBar.MaxValue = Player.Singleton.StartingLives;
            LifeBar.Value = Player.Singleton.Lives;
        }

        RichTextLabel Label = GetNodeOrNull<RichTextLabel>("Count");
        if (Label != null)
        {
            Label.Text = Player.Singleton.Lives.ToString();
        }
    }
}
