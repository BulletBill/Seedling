using Godot;
using System;

public partial class QuitScene : Node
{
    public override void _Ready()
    {
        GetTree().Quit();
    }
}
