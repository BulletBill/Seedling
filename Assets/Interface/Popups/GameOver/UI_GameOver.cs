using Godot;
using System;

public partial class UI_GameOver : Node
{
    public void RestartGame()
    {
        
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }
}
